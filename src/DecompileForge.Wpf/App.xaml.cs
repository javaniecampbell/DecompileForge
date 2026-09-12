using System.IO;
using System.Windows;
using DecompileForge.Comparison;
using DecompileForge.Contracts;
using DecompileForge.Core;
using DecompileForge.Decompilers;
using DecompileForge.Wpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DecompileForge.Wpf;

public partial class App : Application
{
    private readonly IHost _host = Host.CreateDefaultBuilder().ConfigureServices(services =>
    {
        services.AddSingleton<IAuditSink>(_ => new JsonLinesAuditSink(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DecompileForge", "audit.jsonl")));
        services.AddSingleton<IDecompilerBackend, IlSpyBackend>();
        services.AddSingleton<IIlFingerprintService, IlFingerprintService>(); services.AddSingleton<TextSemanticComparer>(); services.AddSingleton<IComparisonEngine, ComparisonEngine>();
        services.AddSingleton<DecompilationOrchestrator>(); services.AddSingleton<MainViewModel>(); services.AddSingleton<MainWindow>();
    }).Build();
    protected override async void OnStartup(StartupEventArgs e) { base.OnStartup(e); await _host.StartAsync(); _host.Services.GetRequiredService<MainWindow>().Show(); }
    protected override async void OnExit(ExitEventArgs e) { await _host.StopAsync(); _host.Dispose(); base.OnExit(e); }
}
