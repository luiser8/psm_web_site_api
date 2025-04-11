using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using psm_web_site_api_project.Payloads;
using psm_web_site_api_project.Responses;
using psm_web_site_api_project.Services.Autenticacion;
using psm_web_site_api_project.Services.StatusResponse;
using psm_web_site_api_project.Utils.JwtUtils;

namespace psm_web_site_api_project.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AutenticacionController(IAutenticacionService autenticacionService) : ControllerBase
{
    /// <summary>Usuarios login</summary>
    /// <remarks>It is possible return usuario login.</remarks>
    /// <param name="usuario">Parameters to login usuario.</param>
    [HttpPost("session")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TokenResponse>> SessionUsuario([FromBody] LoginPayload usuario)
    {
        if (string.IsNullOrEmpty(usuario.Correo) || usuario.Contrasena == "" || usuario.Contrasena == null)
        {
            if (usuario.Correo != "" || usuario.Correo != null)
            {
                if (!usuario.IsValidEmail(usuario?.Correo ?? string.Empty))
                {
                    return BadRequest(new ErrorHandler { Code = 400, Message = "Correo inválido" });
                }
            }
            return BadRequest(new ErrorHandler { Code = 400, Message = "Valores inválidos" });
        }

        try
        {
            var response = await autenticacionService.SessionService(usuario);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorHandler { Code = 400, Message = ex.Message });
        }
    }

    /// <summary>Refresh Token</summary>
    /// <remarks>It is possible user refresh token credentials.</remarks>
    /// <param name="refreshToken">Token actual for refresh.</param>
    [HttpPut("refrescar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TokenResponse>> RefrescarToken(string refreshToken)
    {
        try
        {
            var response = await autenticacionService.RefrescoService(refreshToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorHandler { Code = 400, Message = ex.Message });
        }
    }

    /// <summary>Remove Token</summary>
    /// <remarks>It is possible user remove token credentials.</remarks>
    [HttpPut("remover"), Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> RemoverToken()
    {
        try
        {
            var userData = HttpContext.GetUserTokenData();
            if (userData?.IdUser == null)
            {
                return BadRequest(new ErrorHandler { Code = 400, Message = "Usuario ID no puede ser nulo" });
            }
            // if (userData?.Rol == null || !userData.Rol.Contains("Administrador"))
            // {
            //     return Forbid();
            // }
            if (userData.IsExpired)
            {
                return Unauthorized();
            }
            var response = await autenticacionService.RemoverService(userData.IdUser);

            return Ok(GetStatusResponse.GetStatusResponses(response, "Session", "cierre de sesión"));
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorHandler { Code = 400, Message = ex.Message });
        }
    }
}
