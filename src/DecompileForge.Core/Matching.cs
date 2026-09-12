using DecompileForge.Contracts;

namespace DecompileForge.Core;

public static class ArtifactMatcher
{
    public static IReadOnlyList<(DecompiledFile? Left, DecompiledFile? Right)> Match(IReadOnlyList<DecompiledFile> left, IReadOnlyList<DecompiledFile> right)
    {
        var unmatched = new HashSet<DecompiledFile>(right);
        var pairs = new List<(DecompiledFile?, DecompiledFile?)>();
        foreach (var item in left)
        {
            var match = unmatched.FirstOrDefault(x => SameIdentity(item, x));
            match ??= unmatched.FirstOrDefault(x => string.Equals(item.RelativePath, x.RelativePath, StringComparison.OrdinalIgnoreCase));
            pairs.Add((item, match));
            if (match is not null) unmatched.Remove(match);
        }
        // pairs.AddRange(unmatched?.Select(x => ((DecompiledFile?)null, x)));

        // Cast x to DecompiledFile? so the projected sequence has the same tuple nullability
        pairs.AddRange(unmatched.Select(x => ((DecompiledFile?)null, (DecompiledFile?)x)));
        return pairs;
    }

    private static bool SameIdentity(DecompiledFile a, DecompiledFile b) =>
        a.TypeIdentity is not null && string.Equals(a.TypeIdentity, b.TypeIdentity, StringComparison.Ordinal);
}
