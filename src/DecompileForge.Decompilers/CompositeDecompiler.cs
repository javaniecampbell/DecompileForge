using DecompileForge.Contracts;

namespace DecompileForge.Decompilers;

public sealed class CompositeDecompiler(IEnumerable<IDecompilerBackend> backends) : IDecompilerBackend
{
    public string Name => "composite";
    public bool CanHandle(string path) => backends.Any(x => x.CanHandle(path));
    public Task<DecompilationResult> DecompileAsync(string path, DecompilationOptions options, CancellationToken cancellationToken = default) =>
        (backends.FirstOrDefault(x => x is IlSpyBackend && x.CanHandle(path)) ?? backends.First(x => x.CanHandle(path))).DecompileAsync(path, options, cancellationToken);
}
