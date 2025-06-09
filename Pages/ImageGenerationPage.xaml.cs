using GoyIA.ViewModels;

namespace GoyIA.Pages;

public partial class ImageGenerationPage : ContentPage
{
    public ImageGenerationPage(ImageGenerationViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
} 