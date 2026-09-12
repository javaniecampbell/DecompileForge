using FluentAssertions;
using Xunit;
using Microsoft.Extensions.Logging;
using Moq;
using DecompileForge.Core;

namespace DecompileForge.Comparison.Tests;

public sealed class TextSemanticComparerTests
{
    private static TextSemanticComparer TextSemanticComparer()
    {
        var logger = new Mock<ILogger<TextSemanticComparer>>();
        return new TextSemanticComparer(logger.Object);
    }

    [Fact] public void IgnoresFormattingInSemanticMode() => TextSemanticComparer().Compare("a.cs", "b.cs", "class A{int X=>1;}", "class A { int X => 1; }", true).Should().BeEmpty();
    [Fact] public void DetectsBehavioralSourceChange() => TextSemanticComparer().Compare("a.cs", "b.cs", "class A{int X=>1;}", "class A{int X=>2;}", true).Should().ContainSingle();
}
