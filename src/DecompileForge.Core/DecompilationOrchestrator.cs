using System.Security.Cryptography;
using System.Text.Json;
using DecompileForge.Contracts;

namespace DecompileForge.Core;

public sealed class DecompilationOrchestrator(IEnumerable<IDecompilerBackend> backends, IAuditSink audit)
{
    public async Task<DecompilationResult> RunAsync(string path, DecompilationOptions options, Authorization authorization, CancellationToken ct = default)
    {
        if (!authorization.OwnsOrIsAuthorized) throw new UnauthorizedAccessException("Explicit authorization is required.");
        if (!File.Exists(path)) throw new FileNotFoundException("Assembly not found.", path);
        var backend = backends.FirstOrDefault(x => x.CanHandle(path)) ?? throw new NotSupportedException($"No backend accepts {path}.");
        await audit.WriteAsync("decompilation.started", new { path = Path.GetFileName(path), backend = backend.Name, authorization }, ct);
        var result = await backend.DecompileAsync(path, options, ct);
        await audit.WriteAsync("decompilation.completed", new { result.Backend, result.InputSha256, FileCount = result.Files.Count }, ct);
        return result;
    }
}

public sealed class JsonLinesAuditSink(string path) : IAuditSink, IDisposable
{
    private readonly SemaphoreSlim _gate = new(1, 1);

    public async Task WriteAsync(string action, object payload, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        var line = JsonSerializer.Serialize(new { timestamp = DateTimeOffset.UtcNow, action, payload });
        await _gate.WaitAsync(cancellationToken);
        try { await File.AppendAllTextAsync(path, line + Environment.NewLine, cancellationToken); }
        finally { _gate.Release(); }
    }

    public void Dispose()
    {
        _gate.Dispose();
        GC.SuppressFinalize(this);
    }
}

public static class Hashing
{
    public static async Task<string> Sha256Async(string path, CancellationToken ct = default)
    {
        await using var stream = File.OpenRead(path);
        return Convert.ToHexString(await SHA256.HashDataAsync(stream, ct)).ToLowerInvariant();
    }
}
