using GoyIA.ViewModels;

namespace GoyIA.Pages;

public partial class ImageEditingPage : ContentPage
{
    public ImageEditingPage(ImageEditingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
} 