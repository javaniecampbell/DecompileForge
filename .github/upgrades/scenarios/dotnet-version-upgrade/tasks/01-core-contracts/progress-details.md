# Progress details for 01-core-contracts

Changes applied:
- Updated Directory.Build.props: TargetFramework net8.0 -> net10.0
- Updated src/DecompileForge.Wpf/DecompileForge.Wpf.csproj: TargetFramework net8.0-windows -> net10.0-windows
- Updated global.json: SDK version pinned to 10.0.401 to ensure CLI uses .NET 10

Validation performed:
- Ran `dotnet restore` and `dotnet build` for core projects (DecompileForge.Contracts, DecompileForge.Core).

Result:
- Restore and build succeeded for src/DecompileForge.Contracts and src/DecompileForge.Core targeting net10.0.

Next steps:
- Continue with remaining tasks: upgrade AI and decompilers (02-ai-and-decompilers).

Files modified:
- Directory.Build.props
- src/DecompileForge.Wpf/DecompileForge.Wpf.csproj
- global.json

Notes:
- global.json was updated to pin the .NET SDK to 10.0.401 to ensure the CLI uses the correct SDK during the upgrade.
