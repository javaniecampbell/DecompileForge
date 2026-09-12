param([string]$Configuration='Release')
$ErrorActionPreference = 'Stop'
dotnet restore ./DecompileForge.slnx --locked-mode:$false
dotnet build ./DecompileForge.slnx -c $Configuration --no-restore
dotnet test ./DecompileForge.slnx -c $Configuration --no-build --collect:"XPlat Code Coverage"
dotnet publish ./src/DecompileForge.Cli -c $Configuration -o ./artifacts/cli --no-build
dotnet publish ./src/DecompileForge.Wpf -c $Configuration -r win-x64 --self-contained false -o ./artifacts/wpf
