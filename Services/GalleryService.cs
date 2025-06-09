using System.Text.Json;
using GoyIA.Models;

namespace GoyIA.Services;

public class GalleryService
{
    private const string MetadataFileName = "gallery_metadata.json";
    private readonly string _galleryDirectory;
    private readonly string _metadataFilePath;

    public GalleryService()
    {
        _galleryDirectory = Path.Combine(FileSystem.AppDataDirectory, "Gallery");
        _metadataFilePath = Path.Combine(_galleryDirectory, MetadataFileName);
        
        // Ensure gallery directory exists
        Directory.CreateDirectory(_galleryDirectory);
    }

    public async Task<string> SaveImageAsync(byte[] imageBytes, string prompt, string type)
    {
        try
        {
            var galleryImage = new GalleryImage
            {
                Id = Guid.NewGuid().ToString(),
                FileName = $"{DateTime.Now:yyyyMMdd_HHmmss}_{type}.jpg",
                Type = type,
                Prompt = prompt,
                DateCreated = DateTime.Now,
                FilePath = string.Empty // Will be set below
            };

            // Save the image file
            var imagePath = Path.Combine(_galleryDirectory, galleryImage.FileName);
            await File.WriteAllBytesAsync(imagePath, imageBytes);
            
            galleryImage.FilePath = galleryImage.FileName; // Store relative path

            // Update metadata
            await UpdateMetadataAsync(galleryImage);

            return galleryImage.Id;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to save image: {ex.Message}", ex);
        }
    }

    public async Task<List<GalleryImage>> GetAllImagesAsync()
    {
        try
        {
            var metadata = await LoadMetadataAsync();
            
            // Filter out images where the file no longer exists
            var validImages = new List<GalleryImage>();
            foreach (var image in metadata.Images)
            {
                var fullPath = Path.Combine(_galleryDirectory, image.FilePath);
                if (File.Exists(fullPath))
                {
                    validImages.Add(image);
                }
            }

            // If we found invalid images, update the metadata
            if (validImages.Count != metadata.Images.Count)
            {
                metadata.Images = validImages;
                await SaveMetadataAsync(metadata);
            }

            return validImages.OrderByDescending(i => i.DateCreated).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to load images: {ex.Message}", ex);
        }
    }

    public async Task<byte[]?> GetImageBytesAsync(string imageId)
    {
        try
        {
            var metadata = await LoadMetadataAsync();
            var image = metadata.Images.FirstOrDefault(i => i.Id == imageId);
            
            if (image == null)
                return null;

            var fullPath = Path.Combine(_galleryDirectory, image.FilePath);
            if (!File.Exists(fullPath))
                return null;

            return await File.ReadAllBytesAsync(fullPath);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> DeleteImageAsync(string imageId)
    {
        try
        {
            var metadata = await LoadMetadataAsync();
            var image = metadata.Images.FirstOrDefault(i => i.Id == imageId);
            
            if (image == null)
                return false;

            // Delete the file
            var fullPath = Path.Combine(_galleryDirectory, image.FilePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            // Remove from metadata
            metadata.Images.Remove(image);
            await SaveMetadataAsync(metadata);

            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task UpdateMetadataAsync(GalleryImage newImage)
    {
        var metadata = await LoadMetadataAsync();
        metadata.Images.Add(newImage);
        await SaveMetadataAsync(metadata);
    }

    private async Task<GalleryMetadata> LoadMetadataAsync()
    {
        try
        {
            if (!File.Exists(_metadataFilePath))
            {
                return new GalleryMetadata();
            }

            var json = await File.ReadAllTextAsync(_metadataFilePath);
            var metadata = JsonSerializer.Deserialize<GalleryMetadata>(json);
            return metadata ?? new GalleryMetadata();
        }
        catch
        {
            return new GalleryMetadata();
        }
    }

    private async Task SaveMetadataAsync(GalleryMetadata metadata)
    {
        var json = JsonSerializer.Serialize(metadata, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        await File.WriteAllTextAsync(_metadataFilePath, json);
    }
} 