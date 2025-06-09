using System.Text;
using System.Text.Json;
using GoyIA.Models;

namespace GoyIA.Services;

public class OpenAIService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public OpenAIService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public void SetApiKey(string apiKey)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
    }

    private string GetQualityString()
    {
        var qualityString = Preferences.Get("Image_Quality", "Medium");
        if (Enum.TryParse<ImageQuality>(qualityString, out var quality))
        {
            return quality switch
            {
                ImageQuality.Low => "low",
                ImageQuality.Medium => "medium",
                ImageQuality.High => "high",
                _ => "medium"
            };
        }
        return "medium";
    }

    public async Task<ImageGenerationResponse?> GenerateImageAsync(string prompt)
    {
        var request = new ImageGenerationRequest
        {
            Prompt = prompt,
            Quality = GetQualityString()
        };

        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync("images/generations", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                
                // Add some debugging to see what we're getting back
                System.Diagnostics.Debug.WriteLine($"OpenAI API Response: {responseJson}");
                
                var result = JsonSerializer.Deserialize<ImageGenerationResponse>(responseJson, _jsonOptions);
                
                // Debug the deserialized result
                if (result?.Data?.Any() == true)
                {
                    System.Diagnostics.Debug.WriteLine($"First image B64Json length: '{result.Data.First().B64Json?.Length}'");
                }
                
                return result;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"API request failed with status code: {response.StatusCode}. Response: {errorContent}");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to generate image: {ex.Message}", ex);
        }
    }

    public async Task<ImageGenerationResponse?> EditImageAsync(string prompt, Stream imageStream, string fileName)
    {
        try
        {
            using var formContent = new MultipartFormDataContent();
            
            // Add the image file
            var imageContent = new StreamContent(imageStream);
            imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            formContent.Add(imageContent, "image", fileName);
            
            // Add other parameters
            formContent.Add(new StringContent("gpt-image-1"), "model");
            formContent.Add(new StringContent(prompt), "prompt");
            formContent.Add(new StringContent("1024x1024"), "size");
            formContent.Add(new StringContent(GetQualityString()), "quality");
            
            var response = await _httpClient.PostAsync("images/edits", formContent);
            
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                
                System.Diagnostics.Debug.WriteLine($"OpenAI Edit API Response: {responseJson}");
                
                var result = JsonSerializer.Deserialize<ImageGenerationResponse>(responseJson, _jsonOptions);
                return result;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"API request failed with status code: {response.StatusCode}. Response: {errorContent}");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to edit image: {ex.Message}", ex);
        }
    }
} 