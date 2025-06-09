using System.Globalization;

namespace GoyIA.Converters;

public class GalleryImageSourceConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string filePath && !string.IsNullOrEmpty(filePath))
        {
            var fullPath = Path.Combine(FileSystem.AppDataDirectory, "Gallery", filePath);
            if (File.Exists(fullPath))
            {
                return ImageSource.FromFile(fullPath);
            }
        }
        
        // Return a placeholder or null if file doesn't exist
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
} 