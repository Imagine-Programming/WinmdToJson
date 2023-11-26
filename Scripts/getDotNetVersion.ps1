$relativePath = Get-Item .\WinmdToJson\WinmdToJson.csproj | Resolve-Path -Relative
$xml = [Xml] (Get-Content $relativePath)
$version = ($xml.Project.PropertyGroup.TargetFramework)

# get only version number
$version = $version.Substring($version.Length - 3)
"dotNetVersion=$version" | Out-File -FilePath $env:GITHUB_OUTPUT -Append