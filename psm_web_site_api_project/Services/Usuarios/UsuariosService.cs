using AutoMapper;
using MongoDB.Driver;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Payloads;
using psm_web_site_api_project.Repository.Auditorias;
using psm_web_site_api_project.Repository.Usuarios;
using psm_web_site_api_project.Responses;
using psm_web_site_api_project.Services.Extensiones;
using psm_web_site_api_project.Services.Roles;
using psm_web_site_api_project.Utils.Md5utils;

namespace psm_web_site_api_project.Services.Usuarios;
public class UsuariosService : IUsuariosService
{
    private readonly IUsuariosRepository _usuariosRepository;
    private readonly IRolesService _rolesService;
    private readonly IExtensionesService _extensionesService;
    private readonly IAuditoriasRepository _auditoriasRepository;

    private readonly IMapper _mapper;

    public UsuariosService(IUsuariosRepository usuariosRepository, IRolesService rolesService, IExtensionesService extensionesService, IAuditoriasRepository auditoriasRepository, IMapper mapper)
    {
        _usuariosRepository = usuariosRepository;
        _rolesService = rolesService;
        _extensionesService = extensionesService;
        _auditoriasRepository = auditoriasRepository;
        _mapper = mapper;
    }

    public async Task<List<UsuariosResponse>> SelectUsuariosService()
    {
        try
        {
            return _mapper.Map<List<UsuariosResponse>>(await _usuariosRepository.SelectUsuariosRepository());
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<UsuariosResponse> SelectUsuariosPorIdService(string IdUsuario)
    {
        try
        {
            return _mapper.Map<UsuariosResponse>(await _usuariosRepository.SelectUsuariosPorIdRepository(IdUsuario));
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<List<Auditoria>> SelectUsuariosPorAuditoriaService(string IdUsuario)
    {
        try
        {
            return await _auditoriasRepository.SelectAuditoriasPorUsuarioIdRepository(IdUsuario);
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> PostUsuariosService(UsuarioPayload nuevoUsuario)
    {
        try
        {
            var cursorCorreoUsuario = await _usuariosRepository.SelectUsuariosPorCorreoRepository(nuevoUsuario.Correo ?? string.Empty);

            if (cursorCorreoUsuario != null)
            {
                throw new NotImplementedException("Correo en uso");
            }

            var cursorRol = await _rolesService.SelectRolPorIdService(nuevoUsuario?.Rol ?? string.Empty) ?? throw new NotImplementedException("Rol enviado no existe");
            var cursorExtension = await _extensionesService.GetCursorExtension(nuevoUsuario?.Extensiones?.ToList() ?? []);

            if (cursorExtension.Count <= 0)
            {
                throw new NotImplementedException("Extensiones enviados no existen");
            }
            else
            {
                var compare = nuevoUsuario?.Extensiones?.Intersect(cursorExtension.Select(x => x.IdExtension).ToList()).ToList();
                if (compare?.Count != nuevoUsuario?.Extensiones?.Length)
                {
                    throw new NotImplementedException("Alguna de los Extensiones enviadas no existen");
                }
            }

            var passwordHashCreated = Md5utilsClass.GetMd5(nuevoUsuario?.Contrasena ?? string.Empty);

            var usuario = new Usuario
            {
                Nombres = nuevoUsuario?.Nombres,
                Apellidos = nuevoUsuario?.Apellidos,
                Correo = nuevoUsuario?.Correo,
                Contrasena = passwordHashCreated,
                Rol = cursorRol,
                Extension = cursorExtension,
                FechaCreacion = DateTime.Now
            };

            var response = await _usuariosRepository.PostUsuariosRepository(usuario);

            await _auditoriasRepository.PostAuditoriasRepository(new Auditoria { Tabla = "Usuarios", Accion = "Creación de usuario", IdUsuario = nuevoUsuario?.IdUsuarioIdentity?.ToString() });
            return response.IdUsuario != "" || response.IdUsuario != null;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> PutUsuariosService(string IdUsuario, UsuariosPayloadPut usuario)
    {
        try
        {
            var usuarioExistente = await _usuariosRepository.SelectUsuariosPorIdRepository(IdUsuario) ?? throw new NotImplementedException("Usuario no existe");

            var cursorCorreoUsuario = await _usuariosRepository.SelectUsuariosPorCorreoRepository(usuario?.Correo ?? string.Empty);

            if (usuario?.Correo != null)
                if (cursorCorreoUsuario.IdUsuario != null && cursorCorreoUsuario.Correo == usuario.Correo && cursorCorreoUsuario.IdUsuario != IdUsuario)
                {
                    throw new NotImplementedException("Correo en uso por otro usuario");
                }
                else
                {
                    usuarioExistente.Correo = usuario?.Correo;
                }

            if (usuario?.Nombres != null)
                usuarioExistente.Nombres = usuario.Nombres;
            if (usuario?.Apellidos != null)
                usuarioExistente.Apellidos = usuario.Apellidos;
            if (usuario?.Activo != null)
                usuarioExistente.Activo = (bool)usuario.Activo;
            if (usuario?.Rol != null)
            {
                var cursorRol = await _rolesService.SelectRolPorIdService(usuario.Rol);
                usuarioExistente.Rol = cursorRol;
            }

            if (usuario?.Extensiones?.Length > 0)
            {
                var extensionsToRemove = usuario?.Extensiones.Intersect(usuarioExistente.Extension.Select(x => x.IdExtension).ToList()).ToList();
                var extensionsToSave = usuario?.Extensiones?.Except(extensionsToRemove ?? []).ToList();

                if (extensionsToRemove?.Count > 0)
                {
                    var confirmExtensionDelete = usuarioExistente.Extension.Where(x => !extensionsToRemove.Contains(x.IdExtension)).ToList();
                    if (confirmExtensionDelete?.Count == 0)
                        usuarioExistente.Extension = [];
                    if (confirmExtensionDelete?.Count > 0)
                    {
                        foreach (var extension in confirmExtensionDelete.ToList())
                        {
                            if (extension.Activo)
                                usuarioExistente.Extension.Remove(extension);
                        }
                    }
                }
                if (extensionsToSave?.Count > 0)
                {
                    var nonNullExtensions = extensionsToSave.Where(e => e != null).Cast<string>().ToList();
                    var cursorExtension = await _extensionesService.GetCursorExtension(nonNullExtensions);
                    foreach (var extension in cursorExtension.ToList())
                    {
                        if (extension.Activo)
                            usuarioExistente.Extension.Add(extension);
                    }
                }
            }

            if (usuario?.Contrasena != null)
            {
                var passwordHashCreated = Md5utilsClass.GetMd5(usuario.Contrasena);
                usuarioExistente.Contrasena = passwordHashCreated;
                usuarioExistente.TokenAcceso = null;
                usuarioExistente.TokenRefresco = null;
                usuarioExistente.TokenCreado = null;
                usuarioExistente.TokenExpiracion = null;
            }

            var response = await _usuariosRepository.PutUsuariosRepository(IdUsuario, usuarioExistente);
            await _auditoriasRepository.PostAuditoriasRepository(new Auditoria { Tabla = "Usuarios", Accion = "Modificación de usuario", IdUsuario = IdUsuario });
            return response;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> SetStatusUsuariosService(UsuariosPayloadDelete usuario, bool status)
    {
        try
        {
            var response = await _usuariosRepository.SetStatusUsuariosRepository(usuario?.IdUsuario ?? string.Empty, status);
            await _auditoriasRepository.PostAuditoriasRepository(new Auditoria { Tabla = "Usuarios", Accion = status ? "Activación de Usuario" : "Desactivación de Usuario", IdUsuario = usuario?.IdUsuarioIdentity?.ToString() });
            return response;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> DeleteUsuariosService(UsuariosPayloadDelete usuario)
    {
        try
        {
            var response = await _usuariosRepository.DeleteUsuariosRepository(usuario.IdUsuario ?? string.Empty);
            await _auditoriasRepository.PostAuditoriasRepository(new Auditoria { Tabla = "Usuarios", Accion = "Eliminación de usuario", IdUsuario = usuario?.IdUsuarioIdentity?.ToString() });
            return response;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }
}