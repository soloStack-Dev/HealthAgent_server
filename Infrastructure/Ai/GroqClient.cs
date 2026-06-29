using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MediAgent.Api.Infrastructure.Ai;

public class GroqClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _model;
    private readonly ILogger<GroqClient>? _logger;

    public GroqClient(string apiKey, string model, string baseUrl, int timeoutSeconds = 120, ILogger<GroqClient>? logger = null)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/"),
            Timeout = TimeSpan.FromSeconds(timeoutSeconds)
        };
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        _model = model;
        _logger = logger;
    }

    public async Task<string> ChatAsync(string systemPrompt, string userPrompt, int maxTokens = 2048, float temperature = 0.3f)
    {
        var request = new GroqChatRequest
        {
            Model = _model,
            Messages =
            [
                new GroqMessage { Role = "system", Content = systemPrompt },
                new GroqMessage { Role = "user", Content = userPrompt }
            ],
            Temperature = temperature,
            MaxTokens = maxTokens,
            ResponseFormat = new GroqResponseFormat { Type = "json_object" }
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("chat/completions", content);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GroqChatResponse>();

        _logger?.LogInformation("Groq response received for model {Model}", result?.Model ?? _model);

        return result?.Choices?.FirstOrDefault()?.Message?.Content ?? string.Empty;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}

public class GroqChatRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("messages")]
    public GroqMessage[] Messages { get; set; } = [];

    [JsonPropertyName("temperature")]
    public float Temperature { get; set; }

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; }

    [JsonPropertyName("response_format")]
    public GroqResponseFormat? ResponseFormat { get; set; }
}

public class GroqMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}

public class GroqResponseFormat
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "text";
}

public class GroqChatResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("object")]
    public string? Object { get; set; }

    [JsonPropertyName("created")]
    public long Created { get; set; }

    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("choices")]
    public GroqChoice[]? Choices { get; set; }

    [JsonPropertyName("usage")]
    public GroqUsage? Usage { get; set; }
}

public class GroqChoice
{
    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("message")]
    public GroqResponseMessage? Message { get; set; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; set; }
}

public class GroqResponseMessage
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

public class GroqUsage
{
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; set; }

    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; set; }

    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }
}
