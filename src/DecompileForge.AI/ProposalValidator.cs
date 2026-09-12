using System.Diagnostics;
using DecompileForge.Contracts;

namespace DecompileForge.AI;

public sealed class ProposalValidator : IProposalValidator
{
    public async Task<ProposalValidation> ValidateAsync(GapProposal proposal, string projectDirectory, CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(projectDirectory)) return new(false, false, ["Validation directory does not exist."]);
        var build = await RunAsync("dotnet", ["build", "--no-restore", "--nologo"], projectDirectory, cancellationToken);
        if (build.ExitCode != 0) return new(false, false, [build.Output]);
        var tests = await RunAsync("dotnet", ["test", "--no-build", "--nologo"], projectDirectory, cancellationToken);
        return new(true, tests.ExitCode == 0, tests.ExitCode == 0 ? [] : [tests.Output]);
    }
    private static async Task<(int ExitCode, string Output)> RunAsync(string file, IEnumerable<string> args, string cwd, CancellationToken ct)
    {
        var psi = new ProcessStartInfo(file) { WorkingDirectory = cwd, RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false };
        foreach (var arg in args) psi.ArgumentList.Add(arg); using var p = Process.Start(psi) ?? throw new InvalidOperationException($"Cannot start {file}.");
        var o = p.StandardOutput.ReadToEndAsync(ct); var e = p.StandardError.ReadToEndAsync(ct); await p.WaitForExitAsync(ct); return (p.ExitCode, (await o) + (await e));
    }
}
