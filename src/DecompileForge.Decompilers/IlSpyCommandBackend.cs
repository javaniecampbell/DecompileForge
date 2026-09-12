using System.Diagnostics;
using DecompileForge.Contracts;
using DecompileForge.Core;

namespace DecompileForge.Decompilers;

public sealed class IlSpyCommandBackend(string executable = "ilspycmd") : IDecompilerBackend
{
    public string Name => "ilspy-command";

    private static readonly string[] sourceArray = new[] { ".dll", ".exe" };

    public bool CanHandle(string path) => File.Exists(path) && sourceArray.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);

    public async Task<DecompilationResult> DecompileAsync(string path, DecompilationOptions options, CancellationToken cancellationToken = default)
    {
        var output = options.OutputDirectory ?? Path.Combine(Path.GetTempPath(), "decompileforge", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(output);
        var psi = new ProcessStartInfo(executable) { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false };
        psi.ArgumentList.Add("-p"); psi.ArgumentList.Add("-o"); psi.ArgumentList.Add(output); psi.ArgumentList.Add(path);
        using var process = Process.Start(psi) ?? throw new InvalidOperationException("Could not start ilspycmd.");
        var stderrTask = process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);
        var stderr = await stderrTask;
        if (process.ExitCode != 0) throw new InvalidOperationException($"ilspycmd exited {process.ExitCode}: {stderr}");
        var files = Directory.EnumerateFiles(output, "*.cs", SearchOption.AllDirectories).Select(x => new DecompiledFile(Path.GetRelativePath(output, x), File.ReadAllText(x), null, new Dictionary<string, string>())).ToArray();
        return new(Name, await Hashing.Sha256Async(path, cancellationToken), files, string.IsNullOrWhiteSpace(stderr) ? [] : [stderr], DateTimeOffset.UtcNow);
    }
}
