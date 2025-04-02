using AutoMapper;
using psm_web_site_api_project.Dto;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Repository.Auditorias;
using psm_web_site_api_project.Repository.Autenticacion;
using psm_web_site_api_project.Repository.Usuarios;
using psm_web_site_api_project.Services.Extensiones;
using psm_web_site_api_project.Services.Roles;

namespace psm_web_site_api_project.Services.Autenticacion;
public class AutenticacionService : IAutenticacionService
{
    private readonly IAutenticacionRepository _autenticacionRepository;
    private readonly IUsuariosRepository _usuariosRepository;
    private readonly IRolesService _rolesService;
    private readonly IExtensionesService _extensionesService;
    private readonly IAuditoriasRepository _auditoriasRepository;

    private readonly IMapper _mapper;

    public AutenticacionService(IAutenticacionRepository autenticacionRepository, IUsuariosRepository usuariosRepository, IRolesService rolesService, IExtensionesService extensionesService, IAuditoriasRepository auditoriasRepository, IMapper mapper)
    {
        _autenticacionRepository = autenticacionRepository;
        _usuariosRepository = usuariosRepository;
        _rolesService = rolesService;
        _extensionesService = extensionesService;
        _auditoriasRepository = auditoriasRepository;
        _mapper = mapper;
    }

    public async Task<TokenResponseDto> SessionService(LoginPayloadDto loginPayloadDto)
    {
        try
        {
            var response = await _autenticacionRepository.SessionRepository(loginPayloadDto);
            if (response != null)
            {
                var request = await _usuariosRepository.PutUsuariosRepository(response.IdUsuario ?? string.Empty, response);
                await _auditoriasRepository.PostAuditoriasRepository(new Auditoria { Tabla = "Usuarios", Accion = "Inicio de sesión de usuario", IdUsuario = response.IdUsuario });
            }
            return new TokenResponseDto
            {
                accessToken = response?.TokenAcceso,
                refreshToken = response?.TokenRefresco,
            };
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }
    
    public async Task<TokenResponseDto> RefrescoService(string actualToken)
    {
        try
        {
            var response = await _autenticacionRepository.RefrescoRepository(actualToken);
            var request = await _usuariosRepository.PutUsuariosRepository(response.IdUsuario ?? string.Empty, response);
            await _auditoriasRepository.PostAuditoriasRepository(new Auditoria { Tabla = "Usuarios", Accion = "Refresh de token", IdUsuario = response.IdUsuario });
            return new TokenResponseDto
            {
                accessToken = response?.TokenAcceso,
                refreshToken = response?.TokenRefresco,
            };
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> RemoverService(string usuarioId)
    {
        try
        {
            return await _autenticacionRepository.RemoverRepository(usuarioId);
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> ValidarRepository(string usuarioId, string token)
    {
        try
        {
            return await _autenticacionRepository.ValidarRepository(usuarioId, token);
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }
}