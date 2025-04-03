using psm_web_site_api_project.Dto;
using psm_web_site_api_project.Entities;

namespace psm_web_site_api_project.Services.Autenticacion;

public interface IAutenticacionService
{
    Task<bool> ValidarRepository(string usuarioId, string token);
    Task<TokenResponseDto> SessionService(LoginPayloadDto loginPayloadDto);
    Task<TokenResponseDto> RefrescoService(string refreshToken);
    Task<bool> RemoverService(string usuarioId);
}