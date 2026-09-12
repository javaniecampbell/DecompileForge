namespace DecompileForge.Contracts;

public interface IDecompilerBackend
{
    string Name { get; }
    bool CanHandle(string path);
    Task<DecompilationResult> DecompileAsync(string path, DecompilationOptions options, CancellationToken cancellationToken = default);
}

public interface IComparisonEngine
{
    Task<ComparisonResult> CompareAsync(ComparisonRequest request, CancellationToken cancellationToken = default);
}

public interface IIlFingerprintService
{
    Task<IReadOnlyList<IlFingerprint>> CreateAsync(string assemblyPath, CancellationToken cancellationToken = default);
}

public interface IGapFillService
{
    Task<GapProposal> ProposeAsync(GapFillRequest request, CancellationToken cancellationToken = default);
}

public interface IProposalValidator
{
    Task<ProposalValidation> ValidateAsync(GapProposal proposal, string projectDirectory, CancellationToken cancellationToken = default);
}

public sealed record ProposalValidation(bool Compiles, bool TestsPassed, IReadOnlyList<string> Diagnostics);

public interface IAuditSink
{
    Task WriteAsync(string action, object payload, CancellationToken cancellationToken = default);
}
