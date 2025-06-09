# GoyIA - AI Image Generation & Editing App

A .NET MAUI cross-platform application for generating and editing images using OpenAI's AI models.

## Description

GoyIA is a modern mobile and desktop application built with .NET MAUI that harnesses the power of artificial intelligence for image creation and manipulation. The app integrates with OpenAI's `gpt-image-1` model to provide users with powerful AI-driven image generation and editing capabilities.

## Features

### ✨ **Currently Implemented**
- **AI Image Generation**: Generate high-quality images from text prompts using OpenAI's gpt-image-1 model
- **AI Image Editing**: Edit existing images with AI assistance using the same advanced model
- **Dual Navigation**: Seamless navigation with both bottom tabs and flyout menu
- **Image Input Options**: Select images from device gallery or capture photos with camera
- **Modern UI/UX**: Clean, intuitive interface with enhanced loading states and optimized layouts
- **Cross-Platform Permissions**: Camera and photo library access across all platforms
- **Settings Management**: Secure API key storage and configuration
- **Cross-Platform**: Runs on Android, iOS, Windows, and macOS
- **MVVM Architecture**: Well-structured codebase with proper separation of concerns

### 🚧 **Planned Features**
- **Image Gallery**: Local gallery to save and manage favorite AI-generated/edited images
- **Image Sharing**: Share generated and edited images to social platforms
- **Advanced Generation Options**: Fine-tune image generation parameters
- **Batch Processing**: Edit multiple images simultaneously

## Requirements

- .NET 9.0 or later
- Visual Studio 2022 (recommended) or Visual Studio Code with C# extension
- OpenAI API key (for image generation and editing functionality)

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
│   ├── ImageEditingPage.xaml         # AI image editing interface
│   ├── ImageEditingPage.xaml.cs      # Editing page code-behind
│   ├── SettingsPage.xaml             # App settings interface
│   └── SettingsPage.xaml.cs          # Settings page code-behind
├── ViewModels/
│   ├── ImageGenerationViewModel.cs   # Generation page business logic
│   ├── ImageEditingViewModel.cs      # Editing page business logic
│   └── SettingsViewModel.cs          # Settings page business logic
├── Services/
│   └── OpenAIService.cs               # OpenAI API integration (generation & editing)
├── Models/
│   ├── ImageGenerationRequest.cs     # API request models for generation
│   ├── ImageGenerationResponse.cs    # API response models
│   └── ImageEditRequest.cs           # API request models for editing
├── Converters/
│   └── InvertedBoolConverter.cs       # UI value converters
├── Platforms/
│   ├── Android/AndroidManifest.xml   # Camera & storage permissions
│   └── iOS/Info.plist                # Camera & photo library permissions
├── App.xaml                           # Application-wide resources
├── AppShell.xaml                      # Dual navigation structure (tabs + flyout)
└── MauiProgram.cs                     # Dependency injection setup
```

## Usage

### Navigation
The app features dual navigation for optimal user experience:
- **Bottom Tabs**: Quick switching between "Generate" and "Edit" features
- **Flyout Menu**: Access all features including Settings from anywhere in the app

### Generating Images
1. Open the app (defaults to "Generate" tab)
2. Enter a descriptive text prompt (e.g., "A cute baby sea otter playing in the water")
3. Tap "Generate Image"
4. Wait for the AI to create your image
5. The generated image will appear below the prompt

### Editing Images
1. Navigate to the "Edit" tab or select "Edit Images" from the flyout menu
2. Choose your image source:
   - **📱 Select Image**: Pick from your device's photo gallery
   - **📷 Take Photo**: Capture a new photo with your camera
3. The selected image will appear in full size
4. Enter an editing prompt describing your desired changes (e.g., "Add a beautiful sunset in the background")
5. Tap "✨ Edit Image"
6. Wait for the AI to process your image
7. The edited result will appear below with larger display for better viewing
8. Use placeholder action buttons for future sharing and gallery features

### Managing Settings
1. Access Settings via the flyout menu (hamburger icon)
2. Enter your OpenAI API key
3. Tap "Save Settings"
4. Your API key is stored securely on your device

## Technical Details

- **Framework**: .NET MAUI (.NET 9.0)
- **Architecture**: MVVM (Model-View-ViewModel)
- **AI Integration**: OpenAI gpt-image-1 model for both generation and editing
- **Image Handling**: 
  - MediaPicker for cross-platform image selection and camera capture
  - Base64-encoded images from OpenAI API
  - Stream management for API uploads
- **HTTP Client**: Built-in HttpClient with multipart form data support for image editing
- **Data Storage**: Secure preferences for API key storage
- **Permissions**: Cross-platform camera and photo library access

## API Integration

The app integrates with OpenAI's Image APIs:

### Image Generation
- **Endpoint**: `https://api.openai.com/v1/images/generations`
- **Method**: POST with JSON payload
- **Model**: `gpt-image-1`
- **Response Format**: Base64-encoded JSON

### Image Editing
- **Endpoint**: `https://api.openai.com/v1/images/edit`
- **Method**: POST with multipart form data
- **Model**: `gpt-image-1`
- **Input**: Image file + text prompt
- **Response Format**: Base64-encoded JSON

**Common Settings**:
- **Image Size**: 1024x1024 pixels
- **Quality**: Configurable (currently set to "low" for faster processing)

## Platforms Supported

- ✅ Android (API 21+) - Camera, storage, and media permissions configured
- ✅ iOS (15.0+) - Camera and photo library usage descriptions included
- ✅ Windows (10.0.17763.0+) - File system access for image selection
- ✅ macOS (15.0+) - Full feature support

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
4. Ensure camera and photo permissions are granted on mobile devices
5. Open an issue in this repository for technical problems 