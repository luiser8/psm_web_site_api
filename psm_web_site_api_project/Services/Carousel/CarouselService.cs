using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Payloads;
using psm_web_site_api_project.Repository.Auditorias;
using psm_web_site_api_project.Repository.CarouselRepository;
using psm_web_site_api_project.Repository.Extensiones;
using psm_web_site_api_project.Repository.ImageUpAndDown;
using psm_web_site_api_project.Repository.Usuarios;
using psm_web_site_api_project.Responses;

namespace psm_web_site_api_project.Services.Carousel;
public class CarouselService(ICarouselRepository carouselRepository, IAuditoriasRepository auditoriasRepository, IExtensionesRepository extensionesRepository, IUsuariosRepository usuariosRepository, IImageUpAndDownService imageUpAndDownService) : ICarouselService
{
    public async Task<CarouselResponse> SelectCarouselPorIdExtensionService(string? idExtension)
    {
        try
        {
            var carousels = await carouselRepository.SelectCarouselPorIdExtensionRepository(idExtension);

            if (carousels != null || !string.IsNullOrEmpty(carousels?.IdCarousel))
                carousels?.CarouselCollections?.ForEach((cc) =>
                {
                    if (carousels == null) throw new NotImplementedException("Imagen de logo no existe");
                    carousels.CarouselCollections ??= [];
                });

            if (carousels == null)
                throw new NotImplementedException("No existe un Carousel con este id de extension");

            var existeExtension = await extensionesRepository.SelectExtensionesPorIdRepository(carousels.IdExtension ?? string.Empty) ?? throw new NotImplementedException("Extension Id no existe");
            if (!existeExtension.Activo)
                throw new NotImplementedException("Extension desactivada");

            carousels.CarouselCollections?.ForEach(async cc =>
                {
                    if (cc.Imagen == null) throw new NotImplementedException("Imagen de carousel no existe");
                    var (content, contentType) = await imageUpAndDownService.SelectImageUpAndDownService(cc.Imagen, "Carousel", existeExtension.Nombre);
                    carousels?.CarouselCollections?.AddRange([
                                    new CarouselCollection
                                    {
                                        IdCarouselCollection = cc.IdCarouselCollection,
                                        Nombre = cc.Nombre,
                                        Imagen = $"data:{contentType};base64,{Convert.ToBase64String(content)}",
                                        Link = cc.Nombre?.ToLower(),
                                        Title = cc.Title,
                                        Iframe = cc.Iframe,
                                        Target = true
                                    }
                                ]);
                });

            return new CarouselResponse
            {
                IdCarousel = carousels?.IdCarousel,
                IdExtension = carousels?.IdExtension,
                EsNacional = carousels.EsNacional,
                CarouselCollections = carousels?.CarouselCollections,
                FechaCreacion = carousels.FechaCreacion,
                Activo = carousels.Activo
            };
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> PostCarouselService(CarouselPayload carouselPayload)
    {
        try
        {
            if (carouselPayload == null) throw new ArgumentNullException(nameof(carouselPayload), "Carousel cannot be null");
            var existeExtension = await extensionesRepository.SelectExtensionesPorIdRepository(carouselPayload.IdExtension ?? string.Empty) ?? throw new NotImplementedException("Extension Id no existe");
            if (!existeExtension.Activo)
                throw new NotImplementedException("Extension desactivada");

            var carouselExists = await carouselRepository.SelectCarouselPorIdExtensionRepository(carouselPayload.IdExtension ?? string.Empty);
            if (carouselExists is { Activo: true })
                throw new NotImplementedException("Ya existe un carousel activo para esta extension");

            var usuarioExtension = await usuariosRepository.SelectUsuariosPorIdRepository(carouselPayload.IdUsuarioIdentity ?? string.Empty);
            var usuarioExtensionValid = usuarioExtension.Extension.Where(x => x.IdExtension == carouselPayload.IdExtension);

            if (!usuarioExtensionValid.Any()) throw new NotImplementedException("Extension Id no pertenece al usuario");

            var saveLogoImage = await imageUpAndDownService.PostImageUpAndDownService(carouselPayload.Imagen, "Carousel", existeExtension.Nombre);
            if (string.IsNullOrEmpty(saveLogoImage)) throw new NotImplementedException("Ocurrió un error intentando guardar carousel");

            var esNacional = usuarioExtension.Extension.FirstOrDefault(x => x.IdExtension == carouselPayload.IdExtension)?.EsNacional ?? false;
            var newCarousel = new Entities.Carousel
            {
                IdExtension = carouselPayload.IdExtension,
                EsNacional = esNacional,
                Activo = true,
                CarouselCollections =
                [
                    new CarouselCollection
                    {
                        Nombre = carouselPayload.Nombre,
                        Imagen = saveLogoImage,
                        Link = carouselPayload.Link,
                        Title = carouselPayload.Title,
                        Iframe = carouselPayload.Iframe,
                        Target = carouselPayload.Target,
                        Activo = carouselPayload.Activo
                    }
                ]
            };

            var response = await carouselRepository.PostCarouselRepository(newCarousel);
            await auditoriasRepository.PostAuditoriasRepository(new Auditoria { Tabla = "Carousel", Accion = "Creación de carousel", IdUsuario = carouselPayload?.IdUsuarioIdentity?.ToString() ?? string.Empty });
            return response;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> PutCarouselService(string idCarousel, CarouselPayload carouselPayload)
    {
        try
        {
            if (idCarousel == null || string.IsNullOrEmpty(idCarousel)) throw new ArgumentNullException(nameof(idCarousel), "Carousel Id cannot be null");
            if (carouselPayload == null) throw new ArgumentNullException(nameof(carouselPayload), "Carousel cannot be null");

            var carouselExists = await carouselRepository.SelectCarouselPorIdRepository(idCarousel) ?? throw new NotImplementedException("No existe un Carousel con este id");
            var existeExtension = await extensionesRepository.SelectExtensionesPorIdRepository(carouselPayload.IdExtension ?? string.Empty);

            var usuarioExtension = await usuariosRepository.SelectUsuariosPorIdRepository(carouselPayload.IdUsuarioIdentity ?? string.Empty);
            var usuarioExtensionValid = usuarioExtension.Extension.Where(x => x.IdExtension == carouselPayload.IdExtension);

            if (!usuarioExtensionValid.Any()) throw new NotImplementedException("Extension Id no pertenece al usuario");

            var saveLogoImage = await imageUpAndDownService.PutImageUpAndDownService(carouselPayload.Imagen, "Carousel", existeExtension.Nombre);
            if (string.IsNullOrEmpty(saveLogoImage)) throw new NotImplementedException("Ocurrió un error intentando guardar carousel");

            var newCarousel = new Entities.Carousel
            {
                IdCarousel = idCarousel,
                IdExtension = carouselPayload.IdExtension,
                EsNacional = usuarioExtension.Extension.FirstOrDefault(x => x.IdExtension == carouselPayload.IdExtension)?.EsNacional ?? false,
                Activo = true,
                CarouselCollections =
                [
                    new CarouselCollection
                    {
                        Nombre = carouselPayload.Nombre,
                        Imagen = saveLogoImage,
                        Link = carouselPayload.Link,
                        Title = carouselPayload.Title,
                        Iframe = carouselPayload.Iframe,
                        Target = carouselPayload.Target,
                        Activo = carouselPayload.Activo
                    }
                ]
            };

            var response = await carouselRepository.PutCarouselRepository(idCarousel, newCarousel);
            await auditoriasRepository.PostAuditoriasRepository(new Auditoria { Tabla = "Carousel", Accion = "Actualizar carousel", IdUsuario = carouselPayload?.IdUsuarioIdentity?.ToString() ?? string.Empty });
            return response;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> DeleteCarouselService(CarouselPayload carouselPayload)
    {
        try
        {
            if (string.IsNullOrEmpty(carouselPayload.IdCarousel))
                throw new ArgumentNullException(nameof(carouselPayload), "IdCarousel cannot be null or empty");

            var carouselExists = await carouselRepository.SelectCarouselPorIdRepository(carouselPayload.IdCarousel) ?? throw new NotImplementedException("No existe un Carousel con este id");
            var existeExtension = await extensionesRepository.SelectExtensionesPorIdRepository(carouselExists.IdExtension ?? string.Empty);

            if (carouselExists == null) throw new ArgumentNullException(nameof(carouselPayload), "Carousel cannot be null or empty");

            carouselExists?.CarouselCollections?.ForEach(async (c) =>
            {
                var deleteLogoImage = c.Imagen != null && await imageUpAndDownService.DeleteImageUpAndDownService(c.Imagen, "Carousel", existeExtension.Nombre);
                if (!deleteLogoImage) throw new NotImplementedException("Ocurrió un error intentando eliminar el carousel");
            });

            var response = await carouselRepository.DeleteCarouselRepository(carouselPayload.IdCarousel);
            await auditoriasRepository.PostAuditoriasRepository(new Auditoria { Tabla = "Carousel", Accion = "Eliminación de carousel", IdUsuario = carouselPayload?.IdUsuarioIdentity?.ToString() ?? string.Empty });
            return response;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> AddItemToCarousel(string idExtension, CarouselCollectionPayload newItem)
    {
        if (string.IsNullOrEmpty(idExtension))
            throw new ArgumentNullException(nameof(idExtension), "IdExtension cannot be null or empty");
        if (newItem == null)
            throw new ArgumentNullException(nameof(newItem), "NewItem cannot be null");

        var existeExtension = await extensionesRepository.SelectExtensionesPorIdRepository(idExtension ?? string.Empty) ?? throw new NotImplementedException("Extension Id no existe");
        if (!existeExtension.Activo)
            throw new NotImplementedException("Extension desactivada");

        var carouselExists = await carouselRepository.SelectCarouselPorIdExtensionRepository(idExtension ?? string.Empty);

        if (carouselExists is { Activo: false } && newItem == null)
            throw new ArgumentNullException(nameof(newItem), "Carousel disabled cannot add items");

        var saveLogoImageAdd = await imageUpAndDownService.PostImageUpAndDownService(newItem.Imagen, "Carousel", existeExtension.Nombre);
        if (string.IsNullOrEmpty(saveLogoImageAdd)) throw new NotImplementedException("Ocurrió un error intentando guardar carousel");
        var newCarouselCollection =
            new CarouselCollection
            {
                Nombre = newItem.Nombre,
                Imagen = saveLogoImageAdd,
                Link = newItem.Link,
                Title = newItem.Title,
                Iframe = newItem.Iframe,
                Target = newItem.Target
            };

        return await carouselRepository.AddItemToCarousel(idExtension ?? throw new ArgumentNullException(nameof(newItem), "IdExtension cannot be null"), newCarouselCollection);
    }

    public async Task<bool> RemoveItemFromCarousel(string idExtension, string itemNombreToRemove)
    {
        if (string.IsNullOrEmpty(idExtension))
            throw new ArgumentNullException(nameof(idExtension), "IdExtension cannot be null or empty");
        if (string.IsNullOrEmpty(itemNombreToRemove))
            throw new ArgumentNullException(nameof(itemNombreToRemove), "itemNombreToRemove cannot be null");

        return await carouselRepository.RemoveItemFromCarousel(idExtension, itemNombreToRemove);
    }
}