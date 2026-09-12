using DecompileForge.Contracts;
using FluentAssertions;
using Xunit;
namespace DecompileForge.Core.Tests;

public sealed class ArtifactMatcherTests
{
    [Fact]
    public void MatchesByTypeIdentityBeforePath()
    {
        var left = new DecompiledFile("old/A.cs", "a", "Demo.A", new Dictionary<string, string>()); var right = new DecompiledFile("new/Renamed.cs", "a", "Demo.A", new Dictionary<string, string>());
        var pair = ArtifactMatcher.Match([left], [right]).Single(); pair.Left.Should().Be(left); pair.Right.Should().Be(right);
    }
}
