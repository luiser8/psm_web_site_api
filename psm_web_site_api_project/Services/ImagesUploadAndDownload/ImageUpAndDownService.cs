using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace psm_web_site_api_project.Repository.ImageUpAndDown;
public class ImageUpAndDownService : IImageUpAndDownService
{
    private readonly IConfiguration _configuration;
    private readonly string? _imageFolderPath;

    public ImageUpAndDownService(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        _configuration = configuration;
        var imageFolderPathConfig = _configuration.GetSection("ExtensionsFolders:ImageFolderPath").Value;
        if (string.IsNullOrEmpty(imageFolderPathConfig))
        {
            throw new InvalidOperationException("Image folder path is not configured.");
        }
        //Directory.GetCurrentDirectory()
        _imageFolderPath = Path.Combine(webHostEnvironment.ContentRootPath, imageFolderPathConfig);
        if (!Directory.Exists(_imageFolderPath))
        {
            Directory.CreateDirectory(_imageFolderPath);
        }
    }

    private (bool isValid, string errorMessage) ValidateImage(IFormFile? file, string elementKey)
    {
        // Validación básica del archivo
        if (file == null || file.Length == 0)
        {
            return (false, "No se ha proporcionado un archivo válido");
        }

        // Obtener configuración
        var elementConfig = _configuration.GetSection($"ExtensionsFolders:{elementKey}");

        if (!elementConfig.Exists())
        {
            return (false, $"Configuración no encontrada para el elemento '{elementKey}'");
        }

        // Validar formatos permitidos
        var allowedFormats = _configuration.GetSection($"ExtensionsFolders:Formats").Get<string[]>()?
            .SelectMany(f => f.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .Select(f => f.Trim().ToLower())
            .ToArray() ?? ["jpg", "jpeg", "png"];

        // Mapeo de extensiones a ContentTypes
        var validContentTypes = new Dictionary<string, string[]>
        {
            ["jpg"] = ["image/jpeg"],
            ["jpeg"] = ["image/jpeg"],
            ["png"] = ["image/png"]
        };

        // Obtener extensión del archivo
        var fileName = file.FileName.Trim().ToLower();
        var fileExtension = fileName.Contains('.')
            ? Path.GetExtension(fileName).TrimStart('.').ToLower()
            : fileName;

        // Validar extensión
        if (!allowedFormats.Contains(fileExtension))
        {
            return (false, $"Extensión '.{fileExtension}' no permitida. Formatos aceptados: {string.Join(", ", allowedFormats.Select(f => $".{f}"))}");
        }

        // Validar ContentType como segunda capa de seguridad
        // if (validContentTypes.TryGetValue(fileExtension, out var expectedContentTypes))
        // {
        //     var actualContentType = file.ContentType.ToLower();
        //     if (!expectedContentTypes.Contains(actualContentType))
        //     {
        //         return (false, $"Tipo de contenido no válido para .{fileExtension}. Esperado: {string.Join(" o ", expectedContentTypes)}");
        //     }
        // }

        // Validar tamaño del archivo
        var maxSizeStr = elementConfig["size"] ?? "700kb";
        var maxSize = ParseSize(maxSizeStr);

        if (file.Length > maxSize)
        {
            return (false, $"El archivo excede el tamaño máximo permitido ({maxSizeStr})");
        }

        // Validar dimensiones con ImageSharp
        var requiredDimensions = elementConfig["px"]?.Split('x');
        if (requiredDimensions?.Length != 2) return (true, string.Empty);
        try
        {
            using var image = Image.Load(file.OpenReadStream());
            var requiredWidth = int.Parse(requiredDimensions[0]);
            var requiredHeight = int.Parse(requiredDimensions[1]);

            if (image.Width != requiredWidth || image.Height != requiredHeight)
            {
                return (false, $"Dimensiones incorrectas. Se requiere {requiredWidth}x{requiredHeight} px (Actual: {image.Width}x{image.Height})");
            }
        }
        catch (UnknownImageFormatException)
        {
            return (false, "El archivo no es una imagen válida o está corrupto");
        }
        catch
        {
            return (false, "Error al validar las dimensiones de la imagen");
        }

        return (true, string.Empty);
    }

    private static long ParseSize(string sizeString)
    {
        sizeString = sizeString.ToLower();
        if (sizeString.EndsWith("kb"))
            return long.Parse(sizeString.Replace("kb", "").Trim()) * 1024;
        if (sizeString.EndsWith("mb"))
            return long.Parse(sizeString.Replace("mb", "").Trim()) * 1024 * 1024;
        return long.Parse(sizeString);
    }

    private static string GetContentType(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }

    public async Task<(byte[] content, string contentType)> SelectImageUpAndDownService(string imageName, string elementKey, string? extensionOrSede)
    {
        if (_imageFolderPath == null)
        {
            throw new InvalidOperationException("Image folder path is not configured.");
        }
        var filePath = Path.Combine(_imageFolderPath, extensionOrSede ?? string.Empty, elementKey, imageName);

        try
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Imagen no encontrada");
            }

            var content = await File.ReadAllBytesAsync(filePath);
            var contentType = GetContentType(filePath);

            var result = await OptimizeImageAsync(content, 800, 75).ContinueWith(static t =>
            {
                if (t.IsFaulted)
                {
                    throw new Exception("Error al optimizar la imagen");
                }
                return t.Result;
            });
            contentType = GetContentType(content);
            return (result, contentType);
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<string> PostImageUpAndDownService(IFormFile? image, string elementKey, string? extensionOrSede)
    {
        try
        {
            string? imagePath = null;
            if (image == null) return Path.GetFileName(imagePath) ?? "empty";
            var fileName = Path.GetFileName(image.FileName);
            if (_imageFolderPath == null)
            {
                throw new InvalidOperationException("Image folder path is not configured.");
            }

            var (isValid, errorMessage) = ValidateImage(image, elementKey);
            if (!isValid)
                throw new InvalidOperationException(errorMessage);

            var filePath = Path.Combine($"{_imageFolderPath}/{extensionOrSede}/{elementKey}", fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            imagePath = Path.Combine(_imageFolderPath, fileName);

            return Path.GetFileName(imagePath);
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<string> PutImageUpAndDownService(IFormFile? image, string elementKey, string? extensionOrSede)
    {
        if (image == null || image.Length == 0)
        {
            return "empty";
        }

        // Validar ruta de imágenes configurada
        if (string.IsNullOrEmpty(_imageFolderPath))
        {
            throw new InvalidOperationException("Image folder path is not configured.");
        }

        // Validar la imagen según las reglas del elemento
        var (isValid, errorMessage) = ValidateImage(image, elementKey);
        if (!isValid)
        {
            throw new InvalidOperationException($"Invalid image: {errorMessage}");
        }

        // Crear estructura de directorios si no existe
        var targetDirectory = Path.Combine(_imageFolderPath, extensionOrSede ?? string.Empty, elementKey);
        Directory.CreateDirectory(targetDirectory); // No lanza excepción si ya existe

        // Obtener nombre de archivo seguro
        var fileName = Path.GetFileName(image.FileName);
        var filePath = Path.Combine(targetDirectory, fileName);

        try
        {
            // Eliminar imagen existente si hay una con el mismo nombre
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // Guardar la nueva imagen
            await using (var stream = new FileStream(filePath, FileMode.CreateNew))
            {
                await image.CopyToAsync(stream);
            }

            // Devolver solo el nombre del archivo (sin ruta completa)
            return fileName;
        }
        catch (IOException ioEx)
        {
            throw new IOException($"Error al reemplazar la imagen: {ioEx.Message}", ioEx);
        }
        catch (UnauthorizedAccessException authEx)
        {
            throw new UnauthorizedAccessException($"No tiene permisos para escribir en el directorio: {authEx.Message}", authEx);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error inesperado al procesar la imagen: {ex.Message}", ex);
        }
    }

    public Task<bool> DeleteImageUpAndDownService(string nameImage, string elementKey, string? extensionOrSede)
    {
        try
        {
            if (string.IsNullOrEmpty(nameImage) || string.IsNullOrEmpty(extensionOrSede) || string.IsNullOrEmpty(elementKey))
            {
                throw new InvalidOperationException("Debe enviar los parámetros para eliminar la imagen");
            }

            if (_imageFolderPath == null) return Task.FromResult(false);
            var targetDirectory = Path.Combine(_imageFolderPath, extensionOrSede ?? string.Empty, elementKey);

            var filePath = Path.Combine(targetDirectory, nameImage);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    private static async Task<byte[]> OptimizeImageAsync(byte[] imageBytes, int maxWidth = 800, int quality = 75)
    {
        try
        {
            using var inputStream = new MemoryStream(imageBytes);
            using var image = await Image.LoadAsync(inputStream);

            // Redimensionar si es necesario
            if (image.Width > maxWidth)
            {
                var options = new ResizeOptions
                {
                    Size = new Size(maxWidth, 0),
                    Mode = ResizeMode.Max,
                    Compand = true
                };
                image.Mutate(x => x.Resize(options));
            }

            // Optimizar según formato
            var format = Image.DetectFormat(imageBytes);
            using var outputStream = new MemoryStream();

            await image.SaveAsync(outputStream, GetEncoder(format, quality));
            return outputStream.ToArray();
        }
        catch
        {
            return GetDefaultImage();
        }
    }

    public string GetContentType(byte[] imageBytes)
    {
        try
        {
            var format = Image.DetectFormat(imageBytes);
            return format.DefaultMimeType;
        }
        catch
        {
            return "image/jpeg";
        }
    }

    private static byte[] GetDefaultImage()
    {
        return File.ReadAllBytes("Resources/default-image.jpg");
    }

    private static IImageEncoder GetEncoder(IImageFormat format, int quality)
    {
        return format switch
        {
            _ when format == JpegFormat.Instance => new JpegEncoder { Quality = quality },
            _ when format == PngFormat.Instance => new PngEncoder { CompressionLevel = PngCompressionLevel.BestCompression },
            _ when format == WebpFormat.Instance => new WebpEncoder { Quality = quality },
            _ => new JpegEncoder { Quality = quality }
        };
    }
}