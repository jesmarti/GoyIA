using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using GoyIA.Services;

namespace GoyIA.ViewModels;

public class ImageEditingViewModel : INotifyPropertyChanged
{
    private readonly OpenAIService _openAIService;
    private readonly GalleryService _galleryService;
    private string _prompt = string.Empty;
    private string _statusMessage = string.Empty;
    private bool _isLoading = false;
    private bool _hasError = false;
    private bool _isOriginalImageVisible = false;
    private bool _isEditedImageVisible = false;
    private ImageSource? _originalImageSource;
    private ImageSource? _editedImageSource;
    private Stream? _originalImageStream;
    private string _originalImageFileName = string.Empty;
    private byte[]? _lastEditedImageBytes;

    public ImageEditingViewModel(OpenAIService openAIService, GalleryService galleryService)
    {
        _openAIService = openAIService;
        _galleryService = galleryService;
        SelectImageCommand = new Command(async () => await SelectImageAsync());
        TakePhotoCommand = new Command(async () => await TakePhotoAsync());
        EditImageCommand = new Command(async () => await EditImageAsync(), () => CanEditImage());
        ShareImageCommand = new Command(async () => await ShareImageAsync(), () => CanShareImage());
        SaveToGalleryCommand = new Command(async () => await SaveToGalleryAsync(), () => CanSaveToGallery());
        SaveToDeviceCommand = new Command(async () => await SaveToDeviceAsync(), () => CanSaveToDevice());
    }

    public string Prompt
    {
        get => _prompt;
        set
        {
            if (_prompt != value)
            {
                _prompt = value;
                OnPropertyChanged();
                ((Command)EditImageCommand).ChangeCanExecute();
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
                OnPropertyChanged(nameof(ShowImageArea));
                OnPropertyChanged(nameof(ShowWelcomeMessage));
                ((Command)EditImageCommand).ChangeCanExecute();
                ((Command)ShareImageCommand).ChangeCanExecute();
                ((Command)SaveToGalleryCommand).ChangeCanExecute();
                ((Command)SaveToDeviceCommand).ChangeCanExecute();
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

    public bool IsOriginalImageVisible
    {
        get => _isOriginalImageVisible;
        set
        {
            if (_isOriginalImageVisible != value)
            {
                _isOriginalImageVisible = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowImageArea));
                OnPropertyChanged(nameof(ShowWelcomeMessage));
            }
        }
    }

    public bool IsEditedImageVisible
    {
        get => _isEditedImageVisible;
        set
        {
            if (_isEditedImageVisible != value)
            {
                _isEditedImageVisible = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowImageArea));
                OnPropertyChanged(nameof(ShowWelcomeMessage));
                ((Command)ShareImageCommand).ChangeCanExecute();
                ((Command)SaveToGalleryCommand).ChangeCanExecute();
                ((Command)SaveToDeviceCommand).ChangeCanExecute();
            }
        }
    }

    public ImageSource? OriginalImageSource
    {
        get => _originalImageSource;
        set
        {
            if (_originalImageSource != value)
            {
                _originalImageSource = value;
                OnPropertyChanged();
            }
        }
    }

    public ImageSource? EditedImageSource
    {
        get => _editedImageSource;
        set
        {
            if (_editedImageSource != value)
            {
                _editedImageSource = value;
                OnPropertyChanged();
            }
        }
    }

    public bool ShowImageArea => IsLoading || IsOriginalImageVisible || IsEditedImageVisible;

    public bool ShowWelcomeMessage => !IsLoading && !IsOriginalImageVisible && !IsEditedImageVisible;

    public ICommand SelectImageCommand { get; }
    public ICommand TakePhotoCommand { get; }
    public ICommand EditImageCommand { get; }
    public ICommand ShareImageCommand { get; }
    public ICommand SaveToGalleryCommand { get; }
    public ICommand SaveToDeviceCommand { get; }

    private bool CanEditImage()
    {
        return !IsLoading && !string.IsNullOrWhiteSpace(Prompt) && IsOriginalImageVisible && _originalImageStream != null;
    }

    private bool CanShareImage()
    {
        return !IsLoading && IsEditedImageVisible && _lastEditedImageBytes != null;
    }

    private bool CanSaveToGallery()
    {
        return !IsLoading && IsEditedImageVisible && _lastEditedImageBytes != null;
    }

    private bool CanSaveToDevice()
    {
        return !IsLoading && IsEditedImageVisible && _lastEditedImageBytes != null;
    }

    private async Task SelectImageAsync()
    {
        try
        {
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Select an image to edit"
            });

            if (result != null)
            {
                await LoadImageAsync(result);
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            StatusMessage = $"Error selecting image: {ex.Message}";
        }
    }

    private async Task TakePhotoAsync()
    {
        try
        {
            var result = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
            {
                Title = "Take a photo to edit"
            });

            if (result != null)
            {
                await LoadImageAsync(result);
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            StatusMessage = $"Error taking photo: {ex.Message}";
        }
    }

    private async Task LoadImageAsync(FileResult imageFile)
    {
        try
        {
            // Dispose previous stream if exists
            _originalImageStream?.Dispose();

            // Load the image
            _originalImageStream = await imageFile.OpenReadAsync();
            _originalImageFileName = imageFile.FileName;

            // Create a copy for display (since we need to keep original stream for API)
            var displayStream = await imageFile.OpenReadAsync();
            OriginalImageSource = ImageSource.FromStream(() => displayStream);
            IsOriginalImageVisible = true;
            IsEditedImageVisible = false; // Hide previous edited image
            _lastEditedImageBytes = null; // Clear previous edited image data
            
            HasError = false;
            StatusMessage = "Image loaded successfully! Enter a prompt to edit it.";
            
            ((Command)EditImageCommand).ChangeCanExecute();
        }
        catch (Exception ex)
        {
            HasError = true;
            StatusMessage = $"Error loading image: {ex.Message}";
        }
    }

    private async Task EditImageAsync()
    {
        if (string.IsNullOrWhiteSpace(Prompt) || _originalImageStream == null)
            return;

        IsLoading = true;
        HasError = false;
        StatusMessage = string.Empty;
        IsEditedImageVisible = false; // Hide existing edited image when starting new edit

        try
        {
            // Check if API key is set
            var apiKey = Preferences.Get("OpenAI_API_Key", string.Empty);
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                HasError = true;
                StatusMessage = "Please set your OpenAI API key in Settings";
                return;
            }

            _openAIService.SetApiKey(apiKey);

            // Reset stream position
            _originalImageStream.Position = 0;

            var response = await _openAIService.EditImageAsync(Prompt, _originalImageStream, _originalImageFileName);
            
            if (response?.Data?.Any() == true)
            {
                var base64Json = response.Data.First().B64Json;
                
                if (string.IsNullOrWhiteSpace(base64Json))
                {
                    HasError = true;
                    StatusMessage = "Error: No image data received from OpenAI API.";
                    return;
                }
                
                try
                {
                    _lastEditedImageBytes = Convert.FromBase64String(base64Json);
                    EditedImageSource = ImageSource.FromStream(() => new MemoryStream(_lastEditedImageBytes));
                    IsEditedImageVisible = true;
                    StatusMessage = "Image edited successfully!";
                }
                catch (FormatException)
                {
                    HasError = true;
                    StatusMessage = $"Error: Invalid Base64 image data received.";
                }
            }
            else
            {
                HasError = true;
                StatusMessage = "Failed to edit image. Please try again.";
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ShareImageAsync()
    {
        if (_lastEditedImageBytes == null)
            return;

        try
        {
            // Create a temporary file for sharing
            var fileName = $"GoyIA_Edited_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
            var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
            
            await File.WriteAllBytesAsync(filePath, _lastEditedImageBytes);

            var shareRequest = new ShareFileRequest
            {
                Title = "Share AI Edited Image",
                File = new ShareFile(filePath)
            };

            await Share.RequestAsync(shareRequest);
        }
        catch (Exception ex)
        {
            HasError = true;
            StatusMessage = $"Error sharing image: {ex.Message}";
        }
    }

    private async Task SaveToGalleryAsync()
    {
        if (_lastEditedImageBytes == null)
            return;

        try
        {
            await _galleryService.SaveImageAsync(_lastEditedImageBytes, Prompt, "edited");
            StatusMessage = "Image saved to gallery successfully!";
            HasError = false;
            
            // Show success dialog
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Success", 
                    "The image has been added to the gallery!", 
                    "OK");
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            StatusMessage = $"Error saving to gallery: {ex.Message}";
            
            // Show error dialog
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error", 
                    $"Failed to save image to gallery: {ex.Message}", 
                    "OK");
            }
        }
    }

    private async Task SaveToDeviceAsync()
    {
        if (_lastEditedImageBytes == null)
            return;

        try
        {
            // Create a temporary file for saving
            var fileName = $"GoyIA_Edited_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
            var tempPath = Path.Combine(FileSystem.CacheDirectory, fileName);
            
            // Write to temp file
            await File.WriteAllBytesAsync(tempPath, _lastEditedImageBytes);
            
#if ANDROID || IOS
            // For mobile platforms, save to photo gallery using platform services
            await SaveToPhotoGalleryAsync(_lastEditedImageBytes, fileName);
#else
            // For desktop platforms, save to Pictures folder
            var picturesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var goyiaFolder = Path.Combine(picturesPath, "GoyIA");
            var savePath = Path.Combine(goyiaFolder, fileName);
            
            // Create directory if it doesn't exist
            Directory.CreateDirectory(goyiaFolder);
            
            // Copy file
            File.Copy(tempPath, savePath, true);
            
            // Clean up temp file
            if (File.Exists(tempPath))
                File.Delete(tempPath);
            
            // Show success dialog with path and option to open folder
            if (Application.Current?.MainPage != null)
            {
                var result = await Application.Current.MainPage.DisplayAlert(
                    "Success", 
                    $"Image saved to:\n{savePath}\n\nWould you like to open the folder?", 
                    "Open Folder", 
                    "OK");
                
                if (result)
                {
                    // Open the folder in Windows Explorer
                    try
                    {
                        System.Diagnostics.Process.Start("explorer.exe", goyiaFolder);
                    }
                    catch (Exception ex)
                    {
                        await Application.Current.MainPage.DisplayAlert(
                            "Info", 
                            $"Folder path: {goyiaFolder}", 
                            "OK");
                    }
                }
            }
#endif
        }
        catch (Exception ex)
        {
            HasError = true;
            StatusMessage = $"Error saving to device: {ex.Message}";
            
            // Show error dialog
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error", 
                    $"Failed to save image to device: {ex.Message}", 
                    "OK");
            }
        }
    }

#if ANDROID || IOS
    private async Task SaveToPhotoGalleryAsync(byte[] imageBytes, string fileName)
    {
        // This will need platform-specific implementation
        // For now, let's use the sharing mechanism as a workaround
        var tempPath = Path.Combine(FileSystem.CacheDirectory, fileName);
        await File.WriteAllBytesAsync(tempPath, imageBytes);
        
        // Use the share API to let user save to photos
        await Share.RequestAsync(new ShareFileRequest
        {
            Title = "Save Image",
            File = new ShareFile(tempPath)
        });
        
        // Clean up
        if (File.Exists(tempPath))
            File.Delete(tempPath);
    }
#endif

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Dispose pattern for proper resource cleanup
    public void Dispose()
    {
        _originalImageStream?.Dispose();
    }
} 