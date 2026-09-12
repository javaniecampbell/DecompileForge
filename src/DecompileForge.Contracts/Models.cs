namespace DecompileForge.Contracts;

public enum ArtifactKind { Source, Assembly, Package, Directory }
public enum DifferenceKind { Added, Removed, Modified, Renamed, Equivalent }
public enum DiffLayer { File, Text, Semantic, Api, Il }
public enum RiskLevel { None, Low, Medium, High, Critical }
public enum GapStatus { NotRequested, Proposed, Validated, Rejected, Applied }

public sealed record ArtifactDescriptor(string Path, ArtifactKind Kind, string? Label = null);
public sealed record DecompilationOptions(bool UsePdb = true, bool WholeProject = true, string LanguageVersion = "Latest", string? OutputDirectory = null);
public sealed record DecompiledFile(string RelativePath, string Content, string? TypeIdentity, IReadOnlyDictionary<string, string> Metadata);
public sealed record DecompilationResult(string Backend, string InputSha256, IReadOnlyList<DecompiledFile> Files, IReadOnlyList<string> Warnings, DateTimeOffset CompletedAt);
public sealed record IlFingerprint(string Assembly, string Type, string Method, string Signature, string Sha256, int InstructionCount);
public sealed record Difference(string Id, DifferenceKind Kind, DiffLayer Layer, string? LeftPath, string? RightPath, string Summary, RiskLevel Risk, string? UnifiedDiff = null, IReadOnlyDictionary<string, string>? Evidence = null);
public sealed record ComparisonRequest(ArtifactDescriptor Baseline, ArtifactDescriptor Candidate, bool IncludeText = true, bool IncludeSemantic = true, bool IncludeApi = true, bool IncludeIl = true);
public sealed record ComparisonResult(string Id, IReadOnlyList<Difference> Differences, DateTimeOffset CompletedAt);
public sealed record GapFillRequest(string DifferenceId, string BaselineCode, string CandidateCode, string Evidence, string Language = "csharp");
public sealed record GapProposal(string Id, string DifferenceId, string Explanation, string ReplacementCode, double Confidence, IReadOnlyList<string> Assumptions, IReadOnlyList<string> ValidationSteps, GapStatus Status = GapStatus.Proposed);
public sealed record Authorization(string Operator, string Purpose, string AuthorityReference, bool OwnsOrIsAuthorized, DateTimeOffset AcceptedAt);
