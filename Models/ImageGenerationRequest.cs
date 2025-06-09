using System.Text.Json.Serialization;

namespace GoyIA.Models;

public class ImageGenerationRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = "gpt-image-1";

    [JsonPropertyName("prompt")]
    public string Prompt { get; set; } = string.Empty;

    [JsonPropertyName("n")]
    public int N { get; set; } = 1;

    [JsonPropertyName("size")]
    public string Size { get; set; } = "1024x1024";

    [JsonPropertyName("quality")]
    public string Quality { get; set; } = "low";
} 