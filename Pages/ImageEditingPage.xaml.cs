using GoyIA.ViewModels;
using System.ComponentModel;

namespace GoyIA.Pages;

public partial class ImageEditingPage : ContentPage
{
    private ImageEditingViewModel? _viewModel;

    public ImageEditingPage(ImageEditingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
        
        // Subscribe to property changes to detect when loading starts
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private async void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ImageEditingViewModel.IsLoading) && _viewModel?.IsLoading == true)
        {
            // Small delay to ensure the loading animation is rendered
            await Task.Delay(100);
            await ScrollToLoadingAnimation();
        }
    }

    private async Task ScrollToLoadingAnimation()
    {
        try
        {
            // Scroll to the loading animation section
            await MainScrollView.ScrollToAsync(LoadingAnimationSection, ScrollToPosition.MakeVisible, true);
        }
        catch (Exception)
        {
            // Silently ignore scroll errors
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }
    }
} 