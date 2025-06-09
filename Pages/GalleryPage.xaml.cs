using GoyIA.ViewModels;

namespace GoyIA.Pages;

public partial class GalleryPage : ContentPage
{
    public GalleryPage(GalleryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is GalleryViewModel viewModel)
        {
            await viewModel.InitializeAsync();
        }
    }
} 