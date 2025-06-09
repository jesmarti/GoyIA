using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using GoyIA.Models;
using GoyIA.Services;

namespace GoyIA.ViewModels;

public class GalleryViewModel : INotifyPropertyChanged
{
    private readonly GalleryService _galleryService;
    private bool _isLoading = false;
    private bool _hasError = false;
    private string _statusMessage = string.Empty;
    private GalleryImage? _selectedImage;
    private bool _isModalVisible = false;

    public GalleryViewModel(GalleryService galleryService)
    {
        _galleryService = galleryService;
        Images = new ObservableCollection<GalleryImage>();
        
        LoadImagesCommand = new Command(async () => await LoadImagesAsync());
        ImageTappedCommand = new Command<GalleryImage>(OnImageTapped);
        CloseModalCommand = new Command(CloseModal);
        DeleteImageCommand = new Command<GalleryImage>(async (image) => await DeleteImageAsync(image));
        ShareImageCommand = new Command<GalleryImage>(async (image) => await ShareImageAsync(image));
        RefreshCommand = new Command(async () => await RefreshAsync());
        
        // Ensure initial state is properly set
        OnPropertyChanged(nameof(HasImages));
        OnPropertyChanged(nameof(ShowEmptyState));
    }

    public ObservableCollection<GalleryImage> Images { get; }

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (_isLoading != value)
            {
                _isLoading = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotLoading));
            }
        }
    }

    public bool IsNotLoading => !IsLoading;

    public bool HasError
    {
        get => _hasError;
        set
        {
            if (_hasError != value)
            {
                _hasError = value;
                OnPropertyChanged();
            }
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            if (_statusMessage != value)
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }
    }

    public bool HasImages => Images.Count > 0;

    public bool ShowEmptyState => !IsLoading && !HasImages;

    public GalleryImage? SelectedImage
    {
        get => _selectedImage;
        set
        {
            if (_selectedImage != value)
            {
                _selectedImage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedImageSource));
            }
        }
    }

    public ImageSource? SelectedImageSource
    {
        get
        {
            if (SelectedImage == null) return null;
            
            try
            {
                var fullPath = Path.Combine(FileSystem.AppDataDirectory, "Gallery", SelectedImage.FilePath);
                if (File.Exists(fullPath))
                {
                    return ImageSource.FromFile(fullPath);
                }
            }
            catch
            {
                // Handle errors silently
            }
            return null;
        }
    }

    public bool IsModalVisible
    {
        get => _isModalVisible;
        set
        {
            if (_isModalVisible != value)
            {
                _isModalVisible = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand LoadImagesCommand { get; }
    public ICommand ImageTappedCommand { get; }
    public ICommand CloseModalCommand { get; }
    public ICommand DeleteImageCommand { get; }
    public ICommand ShareImageCommand { get; }
    public ICommand RefreshCommand { get; }

    public async Task InitializeAsync()
    {
        // Ensure UI shows empty state initially while loading
        OnPropertyChanged(nameof(HasImages));
        OnPropertyChanged(nameof(ShowEmptyState));
        
        await LoadImagesAsync();
    }

    private async Task LoadImagesAsync()
    {
        if (IsLoading) return;

        IsLoading = true;
        HasError = false;
        StatusMessage = string.Empty;
        
        // Force update of UI state properties during loading
        OnPropertyChanged(nameof(ShowEmptyState));

        try
        {
            var images = await _galleryService.GetAllImagesAsync();
            
            Images.Clear();
            foreach (var image in images)
            {
                Images.Add(image);
            }

            OnPropertyChanged(nameof(HasImages));
            OnPropertyChanged(nameof(ShowEmptyState));
        }
        catch (Exception ex)
        {
            HasError = true;
            StatusMessage = $"Error loading images: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
            // Ensure UI state is updated after loading completes
            OnPropertyChanged(nameof(ShowEmptyState));
        }
    }

    private async Task RefreshAsync()
    {
        await LoadImagesAsync();
    }

    private void OnImageTapped(GalleryImage image)
    {
        if (image != null)
        {
            SelectedImage = image;
            IsModalVisible = true;
        }
    }

    private void CloseModal()
    {
        IsModalVisible = false;
        SelectedImage = null;
    }

    private async Task DeleteImageAsync(GalleryImage image)
    {
        if (image == null) return;

        // Show confirmation dialog
        if (Application.Current?.MainPage != null)
        {
            var result = await Application.Current.MainPage.DisplayAlert(
                "Delete Image", 
                "Are you sure you want to delete this image? This action cannot be undone.", 
                "Delete", 
                "Cancel");
            
            if (!result)
                return; // User cancelled
        }

        try
        {
            var success = await _galleryService.DeleteImageAsync(image.Id);
            if (success)
            {
                Images.Remove(image);
                OnPropertyChanged(nameof(HasImages));
                OnPropertyChanged(nameof(ShowEmptyState));
                
                // Close modal if the deleted image was selected
                if (SelectedImage?.Id == image.Id)
                {
                    CloseModal();
                }
                
                // Show success message
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Success", 
                        "Image deleted successfully!", 
                        "OK");
                }
            }
            else
            {
                StatusMessage = "Failed to delete image";
                HasError = true;
                
                // Show error dialog
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Error", 
                        "Failed to delete the image. Please try again.", 
                        "OK");
                }
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error deleting image: {ex.Message}";
            HasError = true;
            
            // Show error dialog
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error", 
                    $"An error occurred while deleting the image: {ex.Message}", 
                    "OK");
            }
        }
    }

    private async Task ShareImageAsync(GalleryImage image)
    {
        if (image == null) return;

        try
        {
            var fullPath = Path.Combine(FileSystem.AppDataDirectory, "Gallery", image.FilePath);
            
            if (!File.Exists(fullPath))
            {
                HasError = true;
                StatusMessage = "Image file not found";
                
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Error", 
                        "Image file not found. It may have been deleted.", 
                        "OK");
                }
                return;
            }

            // Create a temporary file for sharing with a user-friendly name
            var fileName = $"GoyIA_{image.Type}_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
            var tempPath = Path.Combine(FileSystem.CacheDirectory, fileName);
            
            // Copy the image to temp directory for sharing
            File.Copy(fullPath, tempPath, true);

            var shareRequest = new ShareFileRequest
            {
                Title = $"Share AI {image.Type} Image",
                File = new ShareFile(tempPath)
            };

            await Share.RequestAsync(shareRequest);
        }
        catch (Exception ex)
        {
            HasError = true;
            StatusMessage = $"Error sharing image: {ex.Message}";
            
            // Show error dialog
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error", 
                    $"Failed to share image: {ex.Message}", 
                    "OK");
            }
        }
    }

    public ImageSource GetImageSource(GalleryImage image)
    {
        try
        {
            var fullPath = Path.Combine(FileSystem.AppDataDirectory, "Gallery", image.FilePath);
            if (File.Exists(fullPath))
            {
                return ImageSource.FromFile(fullPath);
            }
        }
        catch
        {
            // Handle errors silently
        }
        return ImageSource.FromFile("placeholder.png"); // Default placeholder
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 