using System.Text.Json;
using DecompileForge.Comparison;
using DecompileForge.Contracts;
using DecompileForge.Core;
using DecompileForge.Decompilers;

var json = new JsonSerializerOptions { WriteIndented = true };
if (args.Length == 0 || args[0] is "-h" or "--help") { Help(); return 0; }
try
{
    switch (args[0].ToLowerInvariant())
    {
        case "decompile":
            Require(args, 3); var auth = Authorized(args); var output = args[2];
            var orchestrator = new DecompilationOrchestrator([new CompositeDecompiler([new IlSpyBackend(), new MetadataBackend()])], new JsonLinesAuditSink(Path.Combine(output, "audit.jsonl")));
            var result = await orchestrator.RunAsync(args[1], new(true, true, "Latest", output), auth);
            Console.WriteLine(JsonSerializer.Serialize(result, json)); return 0;
        case "compare":
            Require(args, 3); var engine = new ComparisonEngine(new TextSemanticComparer(), new IlFingerprintService());
            var kind = IsAssembly(args[1]) && IsAssembly(args[2]) ? ArtifactKind.Assembly : ArtifactKind.Source;
            var comparison = await engine.CompareAsync(new(new(args[1], kind), new(args[2], kind)));
            Console.WriteLine(JsonSerializer.Serialize(comparison, json)); return comparison.Differences.Count == 0 ? 0 : 2;
        default: Help(); return 1;
    }
}
catch (Exception ex) { Console.Error.WriteLine(JsonSerializer.Serialize(new { error = ex.Message }, json)); return 1; }

static Authorization Authorized(string[] values)
{
    var authority = Value(values, "--authority") ?? throw new ArgumentException("--authority is required.");
    var purpose = Value(values, "--purpose") ?? throw new ArgumentException("--purpose is required.");
    return new(Environment.UserName, purpose, authority, true, DateTimeOffset.UtcNow);
}
static string? Value(string[] values, string key) { var i = Array.IndexOf(values, key); return i >= 0 && i + 1 < values.Length ? values[i + 1] : null; }
static bool IsAssembly(string p) => Path.GetExtension(p).Equals(".dll", StringComparison.OrdinalIgnoreCase) || Path.GetExtension(p).Equals(".exe", StringComparison.OrdinalIgnoreCase);
static void Require(string[] values, int count) { if (values.Length < count) throw new ArgumentException("Missing arguments. Run --help."); }
static void Help() => Console.WriteLine("""
DecompileForge CLI
  decompile <assembly> <output> --authority <reference> --purpose <text>
  compare <baseline> <candidate>
Exit code 2 means differences were detected.
""");
