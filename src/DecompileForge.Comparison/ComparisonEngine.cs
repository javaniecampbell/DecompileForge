using DecompileForge.Contracts;
using Microsoft.Extensions.Logging;

namespace DecompileForge.Comparison;

//public sealed class ComparisonEngine(IIlFingerprintService il) : IComparisonEngine
public sealed class ComparisonEngine(TextSemanticComparer text, IIlFingerprintService il) : IComparisonEngine
{
    public async Task<ComparisonResult> CompareAsync(ComparisonRequest request, CancellationToken cancellationToken = default)
    {
        var differences = new List<Difference>();
        if (request.Baseline.Kind == ArtifactKind.Assembly && request.Candidate.Kind == ArtifactKind.Assembly && request.IncludeIl)
        {
            var left = await il.CreateAsync(request.Baseline.Path, cancellationToken); var right = await il.CreateAsync(request.Candidate.Path, cancellationToken);
            var r = right.ToDictionary(x => (x.Type, x.Method, x.Signature));
            foreach (var l in left)
            {
                if (!r.TryGetValue((l.Type, l.Method, l.Signature), out var rr)) differences.Add(New(DifferenceKind.Removed, $"Removed IL method {l.Type}.{l.Method}", RiskLevel.High));
                else if (l.Sha256 != rr.Sha256) differences.Add(New(DifferenceKind.Modified, $"IL changed in {l.Type}.{l.Method}", RiskLevel.High, new Dictionary<string, string> { ["leftHash"] = l.Sha256, ["rightHash"] = rr.Sha256 }));
            }
            var keys = left.Select(x => (x.Type, x.Method, x.Signature)).ToHashSet(); differences.AddRange(right.Where(x => !keys.Contains((x.Type, x.Method, x.Signature))).Select(x => New(DifferenceKind.Added, $"Added IL method {x.Type}.{x.Method}", RiskLevel.Medium)));
        }
        else if (File.Exists(request.Baseline.Path) && File.Exists(request.Candidate.Path))
        {
            var l = await File.ReadAllTextAsync(request.Baseline.Path, cancellationToken); var r = await File.ReadAllTextAsync(request.Candidate.Path, cancellationToken);
            differences.AddRange(collection: text.Compare(request.Baseline.Path, request.Candidate.Path, l, r, request.IncludeSemantic));
        }
        return new(Guid.NewGuid().ToString("N"), differences, DateTimeOffset.UtcNow);
    }
    private static Difference New(DifferenceKind kind, string summary, RiskLevel risk, IReadOnlyDictionary<string, string>? evidence = null) => new(Guid.NewGuid().ToString("N"), kind, DiffLayer.Il, null, null, summary, risk, null, evidence);
}
