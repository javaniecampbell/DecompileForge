using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DecompileForge.AI;
using DecompileForge.Contracts;

namespace DecompileForge.Wpf.ViewModels;

public partial class MainViewModel(IComparisonEngine comparison, IAuditSink audit) : ObservableObject
{
    [ObservableProperty] private string baselinePath = "";
    [ObservableProperty] private string candidatePath = "";
    [ObservableProperty] private Difference? selectedDifference;
    [ObservableProperty] private string status = "Ready";
    [ObservableProperty] private string provider = "anthropic";
    [ObservableProperty] private string model = "claude-sonnet-4-5";
    [ObservableProperty] private string proposalText = "Select a difference and configure the provider using environment variables.";
    public string[] Providers { get; } = ["anthropic", "openai", "azureopenai", "ollama"];

    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { WriteIndented = true };
    private static readonly string[] sourceArray = new[] { ".dll", ".exe" };

    public ObservableCollection<Difference> Differences { get; } = [];

    [RelayCommand]
    private async Task CompareAsync()
    {
        try {
            Status = "Comparing...";
            Differences.Clear(); var kind = IsAssembly(BaselinePath) && IsAssembly(CandidatePath) ? ArtifactKind.Assembly : ArtifactKind.Source; var result = await comparison.CompareAsync(new(new(BaselinePath, kind), new(CandidatePath, kind))); foreach (var item in result.Differences) Differences.Add(item); Status = $"{Differences.Count} differences"; }
        catch (Exception ex) { Status = ex.Message; }
    }
    [RelayCommand]
    private async Task ProposeAsync()
    {
        if (SelectedDifference is null) { Status = "Select a difference first."; return; }
        try { Status = "Requesting AI proposal..."; using var client = ProviderFactory.Create(new(Provider, Model)); var service = new AiGapFillService(client, audit); var proposal = await service.ProposeAsync(new(SelectedDifference.Id, Read(BaselinePath), Read(CandidatePath), SelectedDifference.UnifiedDiff ?? SelectedDifference.Summary)); ProposalText = JsonSerializer.Serialize(proposal, _jsonOptions); Status = "Proposal received; validation required."; }
        catch (Exception ex) { Status = ex.Message; }
    }
    [RelayCommand]
    private async Task ExportAsync() { var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"decompileforge-{DateTime.UtcNow:yyyyMMddHHmmss}.json"); await File.WriteAllTextAsync(path, JsonSerializer.Serialize(Differences, _jsonOptions)); Status = $"Exported {path}"; }
    private static string Read(string p) => File.Exists(p) && !IsAssembly(p) ? File.ReadAllText(p) : "<binary artifact; use the evidence and hashes only>";
    private static bool IsAssembly(string p) => sourceArray.Contains(Path.GetExtension(p), StringComparer.OrdinalIgnoreCase);
}
