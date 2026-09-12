using DecompileForge.Contracts;
using DiffPlex.Renderer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;

namespace DecompileForge.Comparison;

public sealed class TextSemanticComparer
{
    private readonly ILogger<TextSemanticComparer>? _logger;

    public TextSemanticComparer()
    {
        
    }
    public TextSemanticComparer(ILogger<TextSemanticComparer> logger)
    {
        _logger = logger;
    }

    //public IReadOnlyList<Difference> Compare(string leftPath, string rightPath, string left, string right, bool semantic)
    //{
    //    var normalizedLeft = semantic ? CSharpSyntaxTree.ParseText(left).GetRoot().NormalizeWhitespace().ToFullString() : left;
    //    var normalizedRight = semantic ? CSharpSyntaxTree.ParseText(right).GetRoot().NormalizeWhitespace().ToFullString() : right;
    //    if (normalizedLeft == normalizedRight) return [];
    //    var model = new UnifiedDiffBuilder(new Differ()).BuildDiffModel(normalizedLeft, normalizedRight, leftPath, rightPath);
    //    var patch = string.Join(Environment.NewLine, model.Lines.Select(x => x.Text));
    //    return [new Difference(Guid.NewGuid().ToString("N"), DifferenceKind.Modified, semantic ? DiffLayer.Semantic : DiffLayer.Text, leftPath, rightPath, semantic ? "C# syntax differs after normalization." : "Text differs.", RiskLevel.Medium, patch)];
    //}


    public IReadOnlyList<Difference> Compare(
       string leftPath,
       string rightPath,
       string left,
       string right,
       bool semantic)
    {
        // Guard expensive or unnecessary logging behind IsEnabled to satisfy CA1873
        if (_logger?.IsEnabled(LogLevel.Information) == true)
        {
            #pragma warning disable CA1848 // Use the LoggerMessage delegates
            _logger.LogInformation("Comparing files: {LeftPath} and {RightPath}, semantic: {Semantic}", leftPath, rightPath, semantic);
            #pragma warning restore CA1848 // Use the LoggerMessage delegates
        }

        var normalizedLeft = semantic
            ? CSharpSyntaxTree.ParseText(left).GetRoot().NormalizeWhitespace().ToFullString()
            : left;

        var normalizedRight = semantic
            ? CSharpSyntaxTree.ParseText(right).GetRoot().NormalizeWhitespace().ToFullString()
            : right;

        if (normalizedLeft == normalizedRight)
        {
            return Array.Empty<Difference>();
        }

        var patch = UnidiffRenderer.GenerateUnidiff(
            oldText: normalizedLeft,
            newText: normalizedRight,
            oldFileName: leftPath,
            newFileName: rightPath,
            ignoreWhitespace: false,
            ignoreCase: false,
            contextLines: 3);

        return new[]
        {
            new Difference(
                Guid.NewGuid().ToString("N"),
                DifferenceKind.Modified,
                semantic ? DiffLayer.Semantic : DiffLayer.Text,
                leftPath,
                rightPath,
                semantic ? "C# syntax differs after normalization." : "Text differs.",
                RiskLevel.Medium,
                patch)
        };
    }
}
