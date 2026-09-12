$ErrorActionPreference = 'Stop'
dotnet tool update --global ilspycmd
dotnet tool update --global Microsoft.DotNet.ApiCompat.Tool
dotnet restore ./DecompileForge.slnx
Write-Host 'Bootstrap complete. API keys must be supplied only through environment variables or your secret manager.'
