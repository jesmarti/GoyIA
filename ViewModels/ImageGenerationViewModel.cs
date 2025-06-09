using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using GoyIA.Services;

namespace GoyIA.ViewModels;

public class ImageGenerationViewModel : INotifyPropertyChanged
{
    private readonly OpenAIService _openAIService;
    private string _prompt = string.Empty;
    private string _statusMessage = string.Empty;
    private bool _isLoading = false;
    private bool _hasError = false;
    private bool _isImageVisible = false;
    private ImageSource? _generatedImageSource;

    public ImageGenerationViewModel(OpenAIService openAIService)
    {
        _openAIService = openAIService;
        GenerateImageCommand = new Command(async () => await GenerateImageAsync(), () => CanGenerateImage());
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
                ((Command)GenerateImageCommand).ChangeCanExecute();
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
                ((Command)GenerateImageCommand).ChangeCanExecute();
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

    public bool IsImageVisible
    {
        get => _isImageVisible;
        set
        {
            if (_isImageVisible != value)
            {
                _isImageVisible = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowImageArea));
                OnPropertyChanged(nameof(ShowWelcomeMessage));
            }
        }
    }

    public ImageSource? GeneratedImageSource
    {
        get => _generatedImageSource;
        set
        {
            if (_generatedImageSource != value)
            {
                _generatedImageSource = value;
                OnPropertyChanged();
            }
        }
    }

    public bool ShowImageArea => IsLoading || IsImageVisible;

    public bool ShowWelcomeMessage => !IsLoading && !IsImageVisible;

    public ICommand GenerateImageCommand { get; }

    private bool CanGenerateImage()
    {
        return !IsLoading && !string.IsNullOrWhiteSpace(Prompt);
    }

    private async Task GenerateImageAsync()
    {
        if (string.IsNullOrWhiteSpace(Prompt))
            return;

        IsLoading = true;
        HasError = false;
        StatusMessage = string.Empty;
        IsImageVisible = false; // Hide existing image when starting new generation

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

            var response = await _openAIService.GenerateImageAsync(Prompt);
            
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
                    var imageBytes = Convert.FromBase64String(base64Json);
                    GeneratedImageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                    IsImageVisible = true;
                    StatusMessage = "Image generated successfully!";
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
                StatusMessage = "Failed to generate image. Please try again.";
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

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 