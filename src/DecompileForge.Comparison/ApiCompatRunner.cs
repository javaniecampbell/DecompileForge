using System.Diagnostics;

namespace DecompileForge.Comparison;

public sealed class ApiCompatRunner
{
    public static async Task<(bool Compatible, string Output)> CompareAsync(string baseline, string candidate, bool strict, CancellationToken ct = default)
    {
        var psi = new ProcessStartInfo("apicompat") { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false };
        psi.ArgumentList.Add("--left"); psi.ArgumentList.Add(baseline); psi.ArgumentList.Add("--right"); psi.ArgumentList.Add(candidate); if (strict) psi.ArgumentList.Add("--strict-mode");
        using var p = Process.Start(psi) ?? throw new InvalidOperationException("apicompat was not found. Run scripts/bootstrap.ps1.");
        var stdout = p.StandardOutput.ReadToEndAsync(ct); var stderr = p.StandardError.ReadToEndAsync(ct); await p.WaitForExitAsync(ct);
        return (p.ExitCode == 0, (await stdout) + (await stderr));
    }
}
