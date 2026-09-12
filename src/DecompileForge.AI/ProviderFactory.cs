using System.ClientModel;
using Anthropic;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using OllamaSharp;
using OpenAI;

namespace DecompileForge.AI;

public static class ProviderFactory
{
    public static IChatClient Create(AiOptions options) => options.Provider.ToLowerInvariant() switch
    {
        "anthropic" => new AnthropicClient { ApiKey = Required(options.ApiKey, "ANTHROPIC_API_KEY") }.AsIChatClient(options.Model),
        "openai" => new OpenAIClient(new ApiKeyCredential(Required(options.ApiKey, "OPENAI_API_KEY"))).GetChatClient(options.Model).AsIChatClient(),
        "azureopenai" => new AzureOpenAIClient(new Uri(Required(options.Endpoint, "AZURE_OPENAI_ENDPOINT")), new AzureKeyCredential(Required(options.ApiKey, "AZURE_OPENAI_API_KEY"))).GetChatClient(options.Model).AsIChatClient(),
        "ollama" => new OllamaApiClient(new Uri(options.Endpoint ?? "http://localhost:11434"), options.Model),
        _ => throw new NotSupportedException($"AI provider '{options.Provider}' is not supported.")
    };
    private static string Required(string? value, string env) => value ?? Environment.GetEnvironmentVariable(env) ?? throw new InvalidOperationException($"Set {env}.");
}
public sealed record AiOptions(string Provider, string Model, string? Endpoint = null, string? ApiKey = null);
