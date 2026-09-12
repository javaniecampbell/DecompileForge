using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using DecompileForge.Contracts;
using DecompileForge.Core;

namespace DecompileForge.Decompilers;

public sealed class MetadataBackend : IDecompilerBackend
{
    public string Name => "system-metadata";

    private static readonly string[] sourceArray = new[] { ".dll", ".exe" };

    public bool CanHandle(string path) => File.Exists(path) && sourceArray.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);
    public async Task<DecompilationResult> DecompileAsync(string path, DecompilationOptions options, CancellationToken cancellationToken = default)
    {
        using var stream = File.OpenRead(path); using var pe = new PEReader(stream); var reader = pe.GetMetadataReader();
        var files = reader.TypeDefinitions.Select(h => reader.GetTypeDefinition(h)).Select(t =>
        {
            var ns = reader.GetString(t.Namespace); var name = reader.GetString(t.Name); var id = string.IsNullOrEmpty(ns) ? name : $"{ns}.{name}";
            return new DecompiledFile(id.Replace('.', Path.DirectorySeparatorChar) + ".metadata.txt", $"type {id}; method-count={t.GetMethods().Count}; field-count={t.GetFields().Count}", id, new Dictionary<string, string>());
        }).ToArray();
        return new(Name, await Hashing.Sha256Async(path, cancellationToken), files, ["Metadata backend emits structural metadata, not reconstructed C#."], DateTimeOffset.UtcNow);
    }
}
