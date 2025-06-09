using System.Text.Json.Serialization;

namespace GoyIA.Models;

public class ImageEditRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = "gpt-image-1";

    [JsonPropertyName("prompt")]
    public string Prompt { get; set; } = string.Empty;

    [JsonPropertyName("size")]
    public string Size { get; set; } = "1024x1024";

    [JsonPropertyName("quality")]
    public string Quality { get; set; } = "low";

    // Note: Images will be sent as multipart form data, not in JSON
    // This model is for the form parameters only
} 