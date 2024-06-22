using AutoMapper;
using MongoDB.Driver;
using psm_web_site_api_project.Dto;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Repository.Auditorias;
using psm_web_site_api_project.Repository.Extensiones;
using psm_web_site_api_project.Repository.Roles;
using psm_web_site_api_project.Repository.Usuarios;
using psm_web_site_api_project.Utils.Md5utils;

namespace psm_web_site_api_project.Services.Usuarios;
    public class UsuariosService : IUsuariosService
    {
        private readonly IUsuariosRepository _usuariosRepository;
        private readonly IRolesRepository _rolesRepository;
        private readonly IExtensionesRepository _extensionesRepository;
        private readonly IAuditoriasRepository _auditoriasRepository;

        private readonly IMapper _mapper;

        public UsuariosService(IUsuariosRepository usuariosRepository, IRolesRepository rolesRepository, IExtensionesRepository extensionesRepository, IAuditoriasRepository auditoriasRepository, IMapper mapper)
        {
            _usuariosRepository = usuariosRepository;
            _rolesRepository = rolesRepository;
            _extensionesRepository = extensionesRepository;
            _auditoriasRepository = auditoriasRepository;
            _mapper = mapper;
        }

        public async Task<List<UsuariosResponseDto>> SelectUsuariosService()
        {
            try
            {
                return _mapper.Map<List<UsuariosResponseDto>>(await _usuariosRepository.SelectUsuariosRepository());
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }

        public async Task<UsuariosResponseDto> SelectUsuariosPorIdService(string IdUsuario)
        {
            try
            {
                return _mapper.Map<UsuariosResponseDto>(await _usuariosRepository.SelectUsuariosPorIdRepository(IdUsuario));
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

        public async Task<TokenResponseDto> LoginUsuarioService(LoginPayloadDto loginPayloadDto)
        {
            try
            {
                var response = await _usuariosRepository.LoginUsuarioRepository(loginPayloadDto);
                if (response != null)
                {
                    var request = await _usuariosRepository.PutUsuariosRepository(response.IdUsuario, response);
                    await _auditoriasRepository.PostAuditoriasRepository(new Auditoria { Tabla = "Usuarios", Accion = "Inicio de sesión de usuario", IdUsuario = response.IdUsuario });
                }
                return new TokenResponseDto {
                    accessToken = response.TokenAcceso,
                    refreshToken = response.TokenRefresco,
                };
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }

        public async Task<bool> PostUsuariosService(UsuariosPayloadDto nuevoUsuario)
        {
            try
            {
                var cursorCorreoUsuario = await _usuariosRepository.SelectUsuariosPorCorreoRepository(nuevoUsuario.Correo);

                if (cursorCorreoUsuario != null)
                {
                    throw new NotImplementedException("Correo en uso");
                }

                var filterRoles = Builders<Rol>.Filter.In(r => r.IdRol, nuevoUsuario.Roles);
                var cursorRoles = await _rolesRepository.SelectRolesFilterRepository(filterRoles);
                var roles = cursorRoles.ToList();

                var filterExtension = Builders<Extension>.Filter.In(r => r.IdExtension, nuevoUsuario.Extensiones);
                var cursorExtension = await _extensionesRepository.SelectExtensionesFilterRepository(filterExtension);
                var extensiones = cursorExtension.ToList();

                var passwordHashCreated = Md5utilsClass.GetMD5(nuevoUsuario.Contrasena);

                var usuario = new Usuario
                {
                    Nombres = nuevoUsuario.Nombres,
                    Apellidos = nuevoUsuario.Apellidos,
                    Correo = nuevoUsuario.Correo,
                    Contrasena = passwordHashCreated,
                    Rol = roles,
                    Extension = extensiones,
                    FechaCreacion = DateTime.Now
                };

                var response = await _usuariosRepository.PostUsuariosRepository(usuario);

                await _auditoriasRepository.PostAuditoriasRepository(new Auditoria { Tabla = "Usuarios", Accion = "Creación de usuario", IdUsuario = nuevoUsuario?.IdUsuarioIdentity.ToString() });
                return true;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }

        public async Task<bool> PutUsuariosService(string IdUsuario, UsuariosPayloadPutDto usuario)
        {
            try
            {
                var usuarioExistente = await _usuariosRepository.SelectUsuariosPorIdRepository(IdUsuario) ?? throw new NotImplementedException("Usuario no existe");

                var cursorCorreoUsuario = await _usuariosRepository.SelectUsuariosPorCorreoRepository(usuario?.Correo);

                if (usuario?.Correo != null)
                    if (cursorCorreoUsuario != null && cursorCorreoUsuario.Correo == usuario.Correo && cursorCorreoUsuario.IdUsuario != IdUsuario)
                    {
                        throw new NotImplementedException("Correo en uso por otro usuario");
                    } else {
                        usuarioExistente.Correo = usuario?.Correo;
                    }

                if (usuario?.Nombres != null)
                    usuarioExistente.Nombres = usuario.Nombres;
                if (usuario?.Apellidos != null)
                    usuarioExistente.Apellidos = usuario.Apellidos;
                if (usuario?.Activo != null)
                    usuarioExistente.Activo = (bool)usuario.Activo;

                if (usuario?.Roles?.Length > 0)
                {
                    var filterRoles = Builders<Rol>.Filter.In(r => r.IdRol, usuario.Roles);
                    var cursorRoles = await _rolesRepository.SelectRolesFilterRepository(filterRoles);
                    var roles = cursorRoles.ToList();

                    usuarioExistente.Rol = roles;
                }

                if (usuario?.Extensiones?.Length > 0)
                {
                    var filterExtension = Builders<Extension>.Filter.In(r => r.IdExtension, usuario.Extensiones);
                    var cursorExtension = await _extensionesRepository.SelectExtensionesFilterRepository(filterExtension);
                    var extensiones = cursorExtension.ToList();

                    usuarioExistente.Extension = extensiones;
                }

                if (usuario?.Contrasena != null)
                {
                    var passwordHashCreated = Md5utilsClass.GetMD5(usuario.Contrasena);
                    usuarioExistente.Contrasena = passwordHashCreated;
                    usuarioExistente.TokenAcceso = null;
                    usuarioExistente.TokenRefresco = null;
                    usuarioExistente.TokenCreado = null;
                    usuarioExistente.TokenExpiracion = null;
                }

                await _usuariosRepository.PutUsuariosRepository(IdUsuario, usuarioExistente);
                await _auditoriasRepository.PostAuditoriasRepository(new Auditoria { Tabla = "Usuarios", Accion = "Modificación de usuario", IdUsuario = IdUsuario });
                return true;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }

        public async Task<TokenResponseDto> RefreshTokenService(string actualToken)
        {
            try
            {
                var response = await _usuariosRepository.RefreshTokenRepository(actualToken);
                if (response != null)
                {
                    var request = await _usuariosRepository.PutUsuariosRepository(response.IdUsuario, response);
                    await _auditoriasRepository.PostAuditoriasRepository(new Auditoria { Tabla = "Usuarios", Accion = "Refresh de token", IdUsuario = response.IdUsuario });
                }
                return new TokenResponseDto {
                    accessToken = response.TokenAcceso,
                    refreshToken = response.TokenRefresco,
                };
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }

        public async Task<bool> DeleteUsuariosService(UsuariosPayloadDeleteDto usuario)
        {
            try
            {
                await _usuariosRepository.DeleteUsuariosRepository(usuario.IdUsuario);
                await _auditoriasRepository.PostAuditoriasRepository(new Auditoria { Tabla = "Usuarios", Accion = "Eliminación de usuario", IdUsuario = usuario.IdUsuarioIdentity.ToString() });
                return true;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }
    }