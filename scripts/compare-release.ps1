param([Parameter(Mandatory)]$Baseline,[Parameter(Mandatory)]$Candidate,[switch]$Strict)
$strictArg = if($Strict){'--strict-mode'}else{''}
apicompat --left $Baseline --right $Candidate $strictArg
dotnet run --project ./src/DecompileForge.Cli -- compare $Baseline $Candidate
