using MongoDB.Driver;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Payloads;
using psm_web_site_api_project.Repository.Auditorias;
using psm_web_site_api_project.Repository.Extensiones;

namespace psm_web_site_api_project.Services.Extensiones;
public class ExtensionesService(IExtensionesRepository extensionesRepository, IAuditoriasRepository auditoriasRepository) : IExtensionesService
{
    private readonly IExtensionesRepository _extensionesRepository = extensionesRepository;
    private readonly IAuditoriasRepository _auditoriasRepository = auditoriasRepository;

    public async Task<List<Extension>> SelectExtensionesService()
    {
        try
        {
            return await _extensionesRepository.SelectExtensionesRepository();
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<Extension> SelectExtensionesPorIdService(string idExtension)
    {
        try
        {
            return await _extensionesRepository.SelectExtensionesPorIdRepository(idExtension);
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> PostExtensionesService(ExtensionPayload extension)
    {
        try
        {
            var existeExtension = await _extensionesRepository.SelectExtensionesPorNombreRepository(extension.Nombre ?? string.Empty);

            if (existeExtension != null)
                throw new NotImplementedException("Nombre de extension repetido");

            var nuevaExtension = new Extension
            {
                Nombre = extension.Nombre,
                Descripcion = extension.Descripcion,
            };

            var response = await _extensionesRepository.PostExtensionesRepository(nuevaExtension);

            await _auditoriasRepository.PostAuditoriasRepository(new Auditoria { Tabla = "Extensiones", Accion = "Creación de extension", IdUsuario = extension?.IdUsuarioIdentity?.ToString() });
            return response;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> PutExtensionesService(string idExtension, ExtensionPayload extension)
    {
        try
        {
            var existeExtension = await _extensionesRepository.SelectExtensionesPorIdRepository(idExtension) ?? throw new NotImplementedException("No existe extension");
            if (existeExtension.Nombre == extension.Nombre)
                throw new NotImplementedException("Nombre de extension repetido");

            if (extension?.Nombre != null)
                existeExtension.Nombre = extension.Nombre;
            if (extension?.Descripcion != null)
                existeExtension.Descripcion = extension.Descripcion;
            if (extension?.Activo != null)
                existeExtension.Activo = (bool)extension.Activo;

            await _auditoriasRepository.PostAuditoriasRepository(new Auditoria { Tabla = "Extensiones", Accion = "Actualización de extension", IdUsuario = extension?.IdUsuarioIdentity?.ToString() });
            return await _extensionesRepository.PutExtensionesRepository(idExtension, existeExtension);
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> DeleteExtensionesService(ExtensionPayload extension)
    {
        try
        {
            var response = await _extensionesRepository.DeleteExtensionesRepository(extension?.IdExtension ?? string.Empty);
            await _auditoriasRepository.PostAuditoriasRepository(new Auditoria { Tabla = "Extensiones", Accion = "Eliminación de extension", IdUsuario = extension?.IdUsuarioIdentity?.ToString() });
            return response;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<List<Extension>> GetCursorExtension(List<string> extensiones)
    {
        var filterExtension = Builders<Extension>.Filter.In(r => r.IdExtension, extensiones);
        var cursorExtension = await _extensionesRepository.SelectExtensionesFilterRepository(filterExtension);
        return cursorExtension.ToList();
    }
}