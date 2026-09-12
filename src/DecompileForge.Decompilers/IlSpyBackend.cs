using DecompileForge.Contracts;
using DecompileForge.Core;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.Metadata;

namespace DecompileForge.Decompilers;

//public sealed class IlSpyBackend : IDecompilerBackend
//{
//    public string Name => "ilspy-embedded";
//    public bool CanHandle(string path) => Path.GetExtension(path).Equals(".dll", StringComparison.OrdinalIgnoreCase) || Path.GetExtension(path).Equals(".exe", StringComparison.OrdinalIgnoreCase);

//    public async Task<DecompilationResult> DecompileAsync(string path, DecompilationOptions options, CancellationToken cancellationToken = default)
//    {
//        var settings = new DecompilerSettings(LanguageVersion.Latest) { ThrowOnAssemblyResolveErrors = false, UseDebugSymbols = options.UsePdb };
//        var warnings = new List<string>();
//        if (options.WholeProject && options.OutputDirectory is not null)
//        {
//            Directory.CreateDirectory(options.OutputDirectory);
//            var resolver = new UniversalAssemblyResolver(path, false, null);
//            var module = new PEFile(path);
//            var project = new WholeProjectDecompiler(settings, resolver, resolver, null);
//            project.DecompileProject(module, options.OutputDirectory);
//            var files = Directory.EnumerateFiles(options.OutputDirectory, "*.cs", SearchOption.AllDirectories)
//                .Select(f => new DecompiledFile(Path.GetRelativePath(options.OutputDirectory, f), File.ReadAllText(f), null, new Dictionary<string, string>())).ToArray();
//            return new(Name, await Hashing.Sha256Async(path, cancellationToken), files, warnings, DateTimeOffset.UtcNow);
//        }

//        var decompiler = new CSharpDecompiler(path, settings);
//        var content = decompiler.DecompileWholeModuleAsString();
//        return new(Name, await Hashing.Sha256Async(path, cancellationToken), [new DecompiledFile(Path.GetFileNameWithoutExtension(path) + ".cs", content, null, new Dictionary<string, string>())], warnings, DateTimeOffset.UtcNow);
//    }
//}




public sealed class IlSpyBackend : IDecompilerBackend
{
    public string Name => "ilspy-embedded";

    public bool CanHandle(string path)
    {
        var extension = Path.GetExtension(path);

        return extension.Equals(".dll", StringComparison.OrdinalIgnoreCase)
            || extension.Equals(".exe", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<DecompilationResult> DecompileAsync(
        string path,
        DecompilationOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(options);

        cancellationToken.ThrowIfCancellationRequested();

        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                $"Assembly '{path}' was not found.",
                path);
        }

        var settings = new DecompilerSettings(LanguageVersion.Latest)
        {
            ThrowOnAssemblyResolveErrors = false,
            UseDebugSymbols = options.UsePdb
        };

        var warnings = new List<string>();

        using var module = new PEFile(path);

        var targetFramework = module.Metadata.DetectTargetFrameworkId();

        var resolver = new UniversalAssemblyResolver(
            path,
            throwOnError: false,
            targetFramework);

        if (options.WholeProject && options.OutputDirectory is not null)
        {
            Directory.CreateDirectory(options.OutputDirectory);

            var projectDecompiler = new WholeProjectDecompiler(
                settings,
                resolver,
                projectWriter: null,
                assemblyReferenceClassifier: null,
                debugInfoProvider: null);

            projectDecompiler.DecompileProject(
                module,
                options.OutputDirectory,
                cancellationToken);

            foreach (var error in projectDecompiler.Errors)
            {
                warnings.Add(CSharpDecompiler.GetErrorHeadline(error));
            }

            var files = Directory
                .EnumerateFiles(
                    options.OutputDirectory,
                    "*.cs",
                    SearchOption.AllDirectories)
                .Select(file => new DecompiledFile(
                    Path.GetRelativePath(options.OutputDirectory, file),
                    File.ReadAllText(file),
                    null,
                    new Dictionary<string, string>()))
                .ToArray();

            return new DecompilationResult(
                Name,
                await Hashing.Sha256Async(path, cancellationToken),
                files,
                warnings,
                DateTimeOffset.UtcNow);
        }

        var decompiler = new CSharpDecompiler(
            module,
            resolver,
            settings)
        {
            CancellationToken = cancellationToken
        };

        var content = decompiler.DecompileWholeModuleAsString();

        foreach (var error in decompiler.Errors)
        {
            warnings.Add(CSharpDecompiler.GetErrorHeadline(error));
        }

        return new DecompilationResult(
            Name,
            await Hashing.Sha256Async(path, cancellationToken),
            [
                new DecompiledFile(
                    Path.GetFileNameWithoutExtension(path) + ".cs",
                    content,
                    null,
                    new Dictionary<string, string>())
            ],
            warnings,
            DateTimeOffset.UtcNow);
    }
}
