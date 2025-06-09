using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using GoyIA.Models;

namespace GoyIA.ViewModels;

public class SettingsViewModel : INotifyPropertyChanged
{
    private string _apiKey = string.Empty;
    private string _statusMessage = string.Empty;
    private bool _hasStatusMessage = false;
    private ImageQuality _selectedImageQuality = ImageQuality.Medium;

    public SettingsViewModel()
    {
        SaveSettingsCommand = new Command(SaveSettings);
        LoadSettings();
    }

    public string ApiKey
    {
        get => _apiKey;
        set
        {
            if (_apiKey != value)
            {
                _apiKey = value;
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

    public bool HasStatusMessage
    {
        get => _hasStatusMessage;
        set
        {
            if (_hasStatusMessage != value)
            {
                _hasStatusMessage = value;
                OnPropertyChanged();
            }
        }
    }

    public ImageQuality SelectedImageQuality
    {
        get => _selectedImageQuality;
        set
        {
            if (_selectedImageQuality != value)
            {
                _selectedImageQuality = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsLowQualitySelected));
                OnPropertyChanged(nameof(IsMediumQualitySelected));
                OnPropertyChanged(nameof(IsHighQualitySelected));
            }
        }
    }

    public bool IsLowQualitySelected
    {
        get => SelectedImageQuality == ImageQuality.Low;
        set
        {
            if (value)
                SelectedImageQuality = ImageQuality.Low;
            OnPropertyChanged();
        }
    }

    public bool IsMediumQualitySelected
    {
        get => SelectedImageQuality == ImageQuality.Medium;
        set
        {
            if (value)
                SelectedImageQuality = ImageQuality.Medium;
            OnPropertyChanged();
        }
    }

    public bool IsHighQualitySelected
    {
        get => SelectedImageQuality == ImageQuality.High;
        set
        {
            if (value)
                SelectedImageQuality = ImageQuality.High;
            OnPropertyChanged();
        }
    }

    public ICommand SaveSettingsCommand { get; }

    private void LoadSettings()
    {
        ApiKey = Preferences.Get("OpenAI_API_Key", string.Empty);
        var qualityString = Preferences.Get("Image_Quality", "Medium");
        if (Enum.TryParse<ImageQuality>(qualityString, out var quality))
        {
            SelectedImageQuality = quality;
        }
        else
        {
            SelectedImageQuality = ImageQuality.Medium;
        }
    }

    private void SaveSettings()
    {
        Preferences.Set("OpenAI_API_Key", ApiKey);
        Preferences.Set("Image_Quality", SelectedImageQuality.ToString());
        StatusMessage = "Settings saved successfully!";
        HasStatusMessage = true;

        // Hide the status message after 3 seconds
        _ = Task.Delay(3000).ContinueWith(t =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                HasStatusMessage = false;
            });
        });
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 