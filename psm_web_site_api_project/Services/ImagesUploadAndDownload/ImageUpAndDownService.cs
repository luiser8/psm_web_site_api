using psm_web_site_api_project.Entities;

namespace psm_web_site_api_project.Repository.ImageUpAndDown;
public class ImageUpAndDownService : IImageUpAndDownService
{
    private readonly IConfiguration _configuration;
    private readonly string? _imageFolderPath;

    public ImageUpAndDownService(IConfiguration configuration)
    {
        _configuration = configuration;
        var imageFolderPathConfig = _configuration.GetSection("ImageSettings:ImageFolderPath").Value;
        if (string.IsNullOrEmpty(imageFolderPathConfig))
        {
            throw new InvalidOperationException("Image folder path is not configured.");
        }
        _imageFolderPath = Path.Combine(Directory.GetCurrentDirectory(), imageFolderPathConfig);
    }

    public async Task<byte[]> SelectImageUpAndDownService(string idImage)
    {
        try
        {
            //var headers = await _headerRepository.SelectHeaderPorIdExtensionRepository(idExtension);

            // if (headers != null)
            //     headers?.HeaderCollections?.ForEach(async hc =>
            //     {
            //         if (headers.EsNacional)
            //         {
            //             var extensions = await _extensionesRepository.SelectExtensionesRepository();
            //             headers.HeaderExtensions ??= [];
            //             extensions.ForEach(ext =>
            //             {
            //                 headers.HeaderExtensions.Add(new HeaderExtension
            //                 {
            //                     IdHeaderExtension = ext.IdExtension,
            //                     Nombre = ext.Nombre,
            //                     Link = ext.Nombre?.ToLower(),
            //                     Target = hc.Target
            //                 });
            //             });
            //         }
            //     });

            return [];
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<string> PostImageUpAndDownService(IFormFile image)
    {
        try
        {
            string? imagePath = null;
            if (image != null)
            {
                var fileName = $"{Guid.NewGuid()}-{Path.GetFileName(image.FileName)}";
                if (_imageFolderPath == null)
                {
                    throw new InvalidOperationException("Image folder path is not configured.");
                }
                var filePath = Path.Combine(_imageFolderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                imagePath = Path.Combine(_imageFolderPath, fileName);
            }

            return Path.GetFileName(imagePath) ?? "empty";
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> PutImageUpAndDownService(string idImage, IFormFile image)
    {
        try
        {
            // var existeHeader = await _headerRepository.SelectHeaderPorIdRepository(idHeader) ?? throw new NotImplementedException("No existe header");

            // if (header?.EsNacional != null)
            //     existeHeader.EsNacional = header.EsNacional;
            // if (header?.Logo != null)
            //     existeHeader.Logo = header.Logo;
            // if (header?.Activo != null)
            //     existeHeader.Activo = header.Activo;
            // if (header?.IdExtension != null)
            //     existeHeader.IdExtension = header.IdExtension;
            // if (header?.HeaderCollections != null || header?.HeaderCollections?.Count > 0)
            //     existeHeader.HeaderCollections = header.HeaderCollections;

            // await _auditoriasRepository.PostAuditoriasRepository(new Auditoria { Tabla = "Headers", Accion = "Actualización de header", IdUsuario = header?.IdUsuarioIdentity?.ToString() });
            return true; //await _headerRepository.PutHeaderRepository(idHeader, existeHeader);
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> DeleteImageUpAndDownService(string idImage)
    {
        try
        {
            // var response = await _headerRepository.DeleteHeaderRepository(headerDto.IdHeader);
            // await _auditoriasRepository.PostAuditoriasRepository(new Auditoria { Tabla = "Header", Accion = "Eliminación de header", IdUsuario = headerDto?.IdUsuarioIdentity?.ToString() ?? string.Empty });
            return true;//response;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }
}