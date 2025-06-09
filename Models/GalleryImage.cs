namespace GoyIA.Models;

public class GalleryImage
{
    public string Id { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "generated" or "edited"
    public string Prompt { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
    public string FilePath { get; set; } = string.Empty;
}

public class GalleryMetadata
{
    public List<GalleryImage> Images { get; set; } = new();
} 