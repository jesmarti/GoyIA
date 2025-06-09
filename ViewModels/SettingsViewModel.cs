using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace GoyIA.ViewModels;

public class SettingsViewModel : INotifyPropertyChanged
{
    private string _apiKey = string.Empty;
    private string _statusMessage = string.Empty;
    private bool _hasStatusMessage = false;

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

    public ICommand SaveSettingsCommand { get; }

    private void LoadSettings()
    {
        ApiKey = Preferences.Get("OpenAI_API_Key", string.Empty);
    }

    private void SaveSettings()
    {
        Preferences.Set("OpenAI_API_Key", ApiKey);
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