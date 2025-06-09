# GoyIA - AI Image Generation & Editing App

A .NET MAUI cross-platform application for generating and editing images using OpenAI's AI models.

## Description

GoyIA is a modern mobile and desktop application built with .NET MAUI that harnesses the power of artificial intelligence for image creation and manipulation. The app integrates with OpenAI's `gpt-image-1` model to provide users with powerful AI-driven image generation capabilities.

## Features

### ✨ **Currently Implemented**
- **AI Image Generation**: Generate high-quality images from text prompts using OpenAI's gpt-image-1 model
- **Settings Management**: Secure API key storage and configuration
- **Modern UI/UX**: Clean, intuitive interface with enhanced loading states
- **Cross-Platform**: Runs on Android, iOS, Windows, and macOS
- **MVVM Architecture**: Well-structured codebase with proper separation of concerns

### 🚧 **Planned Features**
- **AI Image Editing**: Edit existing images with AI assistance
- **Image Gallery**: Browse and manage generated/edited images
- **Advanced Generation Options**: Fine-tune image generation parameters

## Requirements

- .NET 9.0 or later
- Visual Studio 2022 (recommended) or Visual Studio Code with C# extension
- OpenAI API key (for image generation functionality)

## Getting Started

1. **Clone this repository**
   ```bash
   git clone [repository-url]
   cd GoyIA
   ```

2. **Open the solution in Visual Studio 2022**

3. **Get your OpenAI API key**
   - Visit [OpenAI Platform](https://platform.openai.com/)
   - Create an account and generate an API key
   - Ensure you have access to the `gpt-image-1` model

4. **Configure the app**
   - Run the application
   - Navigate to Settings (accessible via the flyout menu)
   - Enter your OpenAI API key

5. **Select your target platform and run**
   - Choose Android, iOS, Windows, or macOS
   - Build and run the application

## Project Structure

```
GoyIA/
├── Pages/
│   ├── ImageGenerationPage.xaml      # AI image generation interface
│   ├── ImageGenerationPage.xaml.cs   # Generation page code-behind
│   ├── SettingsPage.xaml             # App settings interface
│   └── SettingsPage.xaml.cs          # Settings page code-behind
├── ViewModels/
│   ├── ImageGenerationViewModel.cs   # Generation page business logic
│   └── SettingsViewModel.cs          # Settings page business logic
├── Services/
│   └── OpenAIService.cs               # OpenAI API integration
├── Models/
│   ├── ImageGenerationRequest.cs     # API request models
│   └── ImageGenerationResponse.cs    # API response models
├── Converters/
│   └── InvertedBoolConverter.cs       # UI value converters
├── App.xaml                           # Application-wide resources
├── AppShell.xaml                      # Navigation structure
└── MauiProgram.cs                     # Dependency injection setup
```

## Usage

### Generating Images
1. Open the app and navigate to "Generate Image"
2. Enter a descriptive text prompt (e.g., "A cute baby sea otter playing in the water")
3. Tap "Generate Image"
4. Wait for the AI to create your image
5. The generated image will appear below the prompt

### Managing Settings
1. Access Settings via the flyout menu (hamburger icon)
2. Enter your OpenAI API key
3. Tap "Save Settings"
4. Your API key is stored securely on your device

## Technical Details

- **Framework**: .NET MAUI (.NET 9.0)
- **Architecture**: MVVM (Model-View-ViewModel)
- **AI Integration**: OpenAI gpt-image-1 model
- **Image Format**: Base64-encoded images from OpenAI API
- **HTTP Client**: Built-in HttpClient with proper header management
- **Data Storage**: Secure preferences for API key storage

## API Integration

The app integrates with OpenAI's Image Generation API:
- **Endpoint**: `https://api.openai.com/v1/images/generations`
- **Model**: `gpt-image-1`
- **Response Format**: Base64-encoded JSON
- **Image Size**: 1024x1024 pixels
- **Quality**: Configurable (currently set to "low" for faster generation)

## Platforms Supported

- ✅ Android (API 21+)
- ✅ iOS (15.0+)
- ✅ Windows (10.0.17763.0+)
- ✅ macOS (15.0+)

## Contributing

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

If you encounter any issues or have questions:
1. Check the Settings page to ensure your API key is correctly configured
2. Verify you have access to the OpenAI gpt-image-1 model
3. Check your internet connection for API requests
4. Open an issue in this repository for technical problems 