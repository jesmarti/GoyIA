using System.Text.Json.Serialization;

namespace GoyIA.Models;

public class ImageGenerationResponse
{
    [JsonPropertyName("created")]
    public long Created { get; set; }

    [JsonPropertyName("data")]
    public List<ImageData> Data { get; set; } = new();
}

public class ImageData
{
    [JsonPropertyName("b64_json")]
    public string B64Json { get; set; } = string.Empty;
} 