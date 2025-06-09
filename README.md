# GoyIA - Aplicación de Generación y Edición de Imágenes con IA

Una aplicación multiplataforma .NET MAUI para generar y editar imágenes utilizando los modelos de IA de OpenAI.

## Descripción

GoyIA es una aplicación moderna para móviles y escritorio construida con .NET MAUI que aprovecha el poder de la inteligencia artificial para la creación y manipulación de imágenes. La aplicación se integra con el modelo `gpt-image-1` de OpenAI para proporcionar a los usuarios potentes capacidades de generación y edición de imágenes impulsadas por IA.

## Características

### ✨ **Actualmente Implementadas**
- **Generación de Imágenes con IA**: Genera imágenes de alta calidad a partir de prompts de texto usando el modelo gpt-image-1 de OpenAI
- **Edición de Imágenes con IA**: Edita imágenes existentes con asistencia de IA usando el mismo modelo avanzado
- **Navegación Dual**: Navegación fluida con pestañas inferiores y menú desplegable
- **Opciones de Entrada de Imagen**: Selecciona imágenes de la galería del dispositivo o captura fotos con la cámara
- **UI/UX Moderna**: Interfaz limpia e intuitiva con estados de carga mejorados y diseños optimizados
- **Permisos Multiplataforma**: Acceso a cámara y biblioteca de fotos en todas las plataformas
- **Multiplataforma**: Funciona en Android, iOS, Windows y macOS
- **Arquitectura MVVM**: Base de código bien estructurada con separación adecuada de responsabilidades
- **Galería de Imágenes**: Galería local para guardar y gestionar imágenes generadas/editadas favoritas
- **Compartir Imágenes**: Compartir imágenes generadas y editadas en plataformas sociales


## Requisitos

- .NET 9.0 o posterior
- Visual Studio 2022 (recomendado) o Visual Studio Code con extensión de C#
- Clave API de OpenAI (para funcionalidad de generación y edición de imágenes)

## Primeros Pasos

1. **Clona este repositorio**

2. **Abre la solución en Visual Studio 2022**

3. **Obtén tu clave API de OpenAI**
   - Visita [OpenAI Platform](https://platform.openai.com/)
   - Crea una cuenta y genera una clave API
   - Asegúrate de tener acceso al modelo `gpt-image-1`

4. **Configura la aplicación**
   - Ejecuta la aplicación
   - Navega a Configuración (accesible a través del menú desplegable)
   - Ingresa tu clave API de OpenAI

5. **Selecciona tu plataforma objetivo y ejecuta**
   - Elige Android, iOS, Windows o macOS
   - Compila y ejecuta la aplicación



## Detalles Técnicos

- **Framework**: .NET MAUI (.NET 9.0)
- **Arquitectura**: MVVM (Model-View-ViewModel)
- **Integración con IA**: Modelo gpt-image-1 de OpenAI para generación y edición
- **Manejo de Imágenes**: 
  - MediaPicker para selección de imágenes y captura de cámara multiplataforma
  - Imágenes codificadas en Base64 de la API de OpenAI
  - Gestión de flujos para cargas de API
- **Cliente HTTP**: HttpClient integrado con soporte para datos de formulario multiparte para edición de imágenes
- **Almacenamiento de Datos**: Preferencias seguras para almacenamiento de claves API
- **Permisos**: Acceso multiplataforma a cámara y biblioteca de fotos

## Integración con API

La aplicación se integra con las APIs de Imagen de OpenAI:

### Generación de Imágenes
- **Endpoint**: `https://api.openai.com/v1/images/generations`
- **Método**: POST con payload JSON
- **Modelo**: `gpt-image-1`
- **Formato de Respuesta**: JSON codificado en Base64

### Edición de Imágenes
- **Endpoint**: `https://api.openai.com/v1/images/edit`
- **Método**: POST con datos de formulario multiparte
- **Modelo**: `gpt-image-1`
- **Entrada**: Archivo de imagen + prompt de texto
- **Formato de Respuesta**: JSON codificado en Base64

**Configuraciones Comunes**:
- **Tamaño de Imagen**: 1024x1024 píxeles
- **Calidad**: Configurable (actualmente establecido en "low" para procesamiento más rápido)