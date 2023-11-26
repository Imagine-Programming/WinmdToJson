using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;
using Win32MetadataJsonGen.Decoders;
using Win32MetadataJsonGen.Extensions;
using Win32MetadataJsonGen.Models;
using Win32MetadataJsonGen.Types;

namespace Win32MetadataJsonGen;

internal class Reader
{
    private readonly MetadataReader _reader;
    private readonly TypeDecoder _typeDecoder;
    private readonly AttributeDecoder _attributeDecoder;
    private readonly Dictionary<string, Reference<BaseType>> _registry = [];
    private readonly List<Namespace> _namespaces = [];

    public Reader(MetadataReader reader)
    {
        _reader = reader;
        _typeDecoder = new TypeDecoder(_registry);
        _attributeDecoder = new AttributeDecoder(_registry);
    }

    public ReadResult Read(List<string> namespaces)
    {
        // load primitive types before hand
        foreach (var primitiveType in Enum.GetValues(typeof(PrimitiveTypeCode)))
        {
            var type = new PrimitiveType((PrimitiveTypeCode)primitiveType);
            _registry[type.Name] = new Reference<BaseType>() { Value = type };
        }

        var namespacesToRead = GetNamespaces()
            .Select(n => new KeyValuePair<string, NamespaceDefinition>(n.GetFullName(_reader), n))
            .Where(n => namespaces.Count == 0 || namespaces.Contains(n.Key))
            .ToList();

        foreach (var @namespace in namespacesToRead)
            ReadNamespace(@namespace);

        return new ReadResult(_namespaces, _registry);
    }

    private void ReadNamespace(KeyValuePair<string, NamespaceDefinition> namespaceDefinition)
    {
        var typeHandles = namespaceDefinition.Value.TypeDefinitions;

        var @namespace = new Namespace(namespaceDefinition.Key);

        foreach (var typeHandle in typeHandles)
        {
            var type = _reader.GetTypeDefinition(typeHandle);
            var typeName = _reader.GetString(type.Name);
            var fullTypeName = Utils.GetFullTypeName(@namespace.Name, _reader.GetString(type.Name));

            // empty type that can be skipped
            if (typeName == "<Module>") continue;

            var reference = _registry.GetOrCreate(fullTypeName);

            reference.Value = type.ToType(@namespace.Name, typeName, _reader, _typeDecoder);

            ProcessBaseType(reference.Value, type);

            if (reference.Value is EnumType)
                ProcessEnumType(reference, type);
            else if (reference.Value is StructType)
                ProcessObjectType(reference, type);
            else if (reference.Value is UnionType)
                ProcessObjectType(reference, type);
            else if (reference.Value is ObjectType)
                ProcessObjectType(reference, type);
            else if (reference.Value is FunctionPointerType)
                ProcessFunctionPointerType(reference, type);
            else if (reference.Value is ComType)
                ProcessObjectType(reference, type);
            else
                continue;

            @namespace.Types.Add(reference.Value);
        }

        _namespaces.Add(@namespace);
    }

    private void ProcessObjectType(Reference<BaseType> reference, TypeDefinition typeDefinition)
    {
        ArgumentNullException.ThrowIfNull(reference.Value, nameof(reference));
        var type = (ObjectType)reference.Value;

        type.Types = ProcessTypes(typeDefinition.GetNestedTypes(), type);
        type.Methods = ProcessMethods(typeDefinition.GetMethods());
        type.Fields = ProcessFields(typeDefinition.GetFields());
    }

    private void ProcessEnumType(Reference<BaseType> reference, TypeDefinition typeDefinition)
    {
        ArgumentNullException.ThrowIfNull(reference.Value, nameof(reference));
        var type = (EnumType)reference.Value;

        var fields = new List<Field>();

        var fieldHandles = typeDefinition
            .GetFields()
            .ToList();

        int startIndex;
        var firstField = ProcessField(fieldHandles[0]);

        // first field should always be present, is a hidden field containing the primitive type of the enum
        type.Type = firstField!.Type!;
        type.FieldAttributes = firstField!.Attributes;

        if (fieldHandles.Count > 1)
        {
            // prefer the second field for the fieldattributes as the first one can differ from the rest
            var secondField = ProcessField(fieldHandles[1]);
            type.FieldAttributes = secondField!.Attributes;

            secondField.Attributes = null;
            secondField.Type = null;
            fields.Add(secondField!);

            startIndex = 2;
        }
        else
        {
            return;
        }

        foreach (var fieldHandle in fieldHandles[startIndex..])
        {
            var field = ProcessField(fieldHandle);
            // set to null to prevent large json file size, as they are the same for all fields of the enum, these are set at enumtype level
            field.Attributes = null;
            field.Type = null;
            fields.Add(field);
        }

        if (fields.Count > 0)
            type.Fields = fields;
    }

    private void ProcessFunctionPointerType(Reference<BaseType> reference, TypeDefinition typeDefinition)
    {
        ArgumentNullException.ThrowIfNull(reference.Value, nameof(reference));
        var type = (FunctionPointerType)reference.Value;

        // delegate contains 2 methods, .ctor and Invoke, we use the Invoke method to get the type info
        var methodHandles = typeDefinition.GetMethods();

        Debug.Assert(methodHandles.Count == 2);

        var method = ProcessMethods(methodHandles.Skip(1))![0];
        type.CustomAttributes = method.CustomAttributes;
        type.Parameters = method.Parameters;
        type.ReturnType = method.ReturnType;
    }

    private List<BaseType>? ProcessTypes(IEnumerable<TypeDefinitionHandle> typeHandles, BaseType baseType)
    {
        var types = new List<BaseType>();

        foreach (var typeHandle in typeHandles)
        {
            if (typeHandle.IsNil) continue;

            var nestedTypeDefinition = _reader.GetTypeDefinition(typeHandle);
            var nestedTypeName = _reader.GetString(nestedTypeDefinition.Name);
            var nestedType = nestedTypeDefinition.ToType(baseType.Namespace, nestedTypeName, _reader, _typeDecoder);

            nestedType.CustomAttributes = nestedTypeDefinition
                .GetCustomAttributes()
                .ToAttributeType(_attributeDecoder, _reader);

            var nestedTypeReference = _registry.GetOrCreate(Utils.GetFullTypeName(baseType.GetFullName(), nestedTypeName));
            nestedTypeReference.Value = nestedType;

            // handles both struct and union cases
            if (nestedType is ObjectType)
            {
                ProcessObjectType(nestedTypeReference, nestedTypeDefinition);
            }
            else
            {
                throw new NotImplementedException("Type not supported");
            }

            types.Add(nestedTypeReference.Value);
        }

        if (types.Count > 0)
            return types;

        return null;
    }

    private List<FunctionType>? ProcessMethods(IEnumerable<MethodDefinitionHandle> methodHandles)
    {
        var methods = new List<FunctionType>();

        foreach (var methodHandle in methodHandles)
        {
            var method = _reader.GetMethodDefinition(methodHandle);
            var methodName = _reader.GetString(method.Name);
            var importAttributes = method.GetImport().Attributes.GetFlagsAsStrings();
            var parameters = method.GetParameters()
                .Select(_reader.GetParameter)
                .OrderBy(p => p.SequenceNumber)
                .ToList();
            var signature = method.DecodeSignature(_typeDecoder, null);

            var function = new FunctionType(methodName, null)
            {
                ImportAttributes = method.GetImport()
                    .Attributes
                    .GetFlagsAsStrings()
                    ?.ToList(),
                MethodAttributes = method.Attributes
                    .GetFlagsAsStrings()
                    ?.ToList(),
                ImplAttributes = method.ImplAttributes
                    .GetFlagsAsStrings()
                    ?.ToList(),
                CustomAttributes = method.GetCustomAttributes()
                    .ToAttributeType(_attributeDecoder, _reader)
            };

            if (parameters.Count > 0)
                function.Parameters = [];

            foreach (var parameter in parameters)
            {
                if (parameter.SequenceNumber == 0) continue;
                var parameterName = _reader.GetString(parameter.Name);
                var parameterType = signature.ParameterTypes[parameter.SequenceNumber - 1];

                var functionParameter = new FunctionParameter(parameterName, parameterType)
                {
                    Attributes = parameter.Attributes
                        .GetFlagsAsStrings()
                        ?.ToList(),
                };

                if (parameter.Attributes.HasFlag(ParameterAttributes.HasDefault))
                {
                    var parameterConstantHandle = parameter.GetDefaultValue();

                    if (parameterConstantHandle.IsNil)
                        throw new InvalidOperationException("Unreadable default parameter value");

                    var parameterConstant = _reader.GetConstant(parameterConstantHandle);
                    functionParameter.Value = parameterConstant.GetValue(_reader);
                }
                function.Parameters!.Add(functionParameter);
            }
            methods.Add(function);
        }

        if (methods.Count > 0)
            return methods;

        return null;
    }

    private List<Field>? ProcessFields(IEnumerable<FieldDefinitionHandle> fieldHandles)
    {
        var fields = new List<Field>();

        foreach (var field in fieldHandles)
        {
            fields.Add(ProcessField(field));
        }

        if (fields.Count > 0)
            return fields;

        return null;
    }

    private void ProcessBaseType(BaseType type, TypeDefinition typeDefinition)
    {
        type.Attributes = typeDefinition.Attributes
            .GetFlagsAsStrings()
            ?.Distinct()
            ?.ToList();
        type.CustomAttributes = typeDefinition
            .GetCustomAttributes()
            .ToAttributeType(_attributeDecoder, _reader);
    }

    private Field ProcessField(FieldDefinitionHandle fieldHandle)
    {
        var fieldDefinition = _reader.GetFieldDefinition(fieldHandle);
        var fieldName = _reader.GetString(fieldDefinition.Name);
        var fieldType = fieldDefinition.DecodeSignature(_typeDecoder, null);

        var field = new Field(fieldName, fieldType)
        {
            Attributes = fieldDefinition.Attributes
                .GetFlagsAsStrings()
                ?.ToList(),
            CustomAttributes = fieldDefinition
                .GetCustomAttributes()
                .ToAttributeType(_attributeDecoder, _reader),
        };

        if (fieldDefinition.Attributes.HasFlag(FieldAttributes.HasDefault))
        {
            var constantHandle = fieldDefinition.GetDefaultValue();

            if (constantHandle.IsNil)
                throw new InvalidOperationException("Unable to process value");

            var constantDefinition = _reader.GetConstant(constantHandle);

            field.Value = constantDefinition.GetValue(_reader);
        }

        return field;
    }

    private List<NamespaceDefinition> GetNamespaces()
    {
        var process = true;
        var index = 0;
        var namespaces = new List<NamespaceDefinition>()
        {
            _reader.GetNamespaceDefinitionRoot()
        };

        while (process)
        {
            for (var i = index; i < namespaces.Count; i++)
            {
                var definition = namespaces[i];

                var subDefinitionHandles = definition.NamespaceDefinitions;

                foreach (var definitionHandle in subDefinitionHandles)
                {
                    namespaces.Add(_reader.GetNamespaceDefinition(definitionHandle));
                }

                index++;
            }

            if (index == namespaces.Count)
                process = false;
        }

        return namespaces;
    }
}
