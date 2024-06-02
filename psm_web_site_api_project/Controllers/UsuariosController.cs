using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using psm_web_site_api_project.Dto;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Services.Usuarios;
using psm_web_site_api_project.Utils.GetIdentities;

namespace psm_web_site_api_project.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController(IUsuariosService usuariosService) : ControllerBase
    {
        private readonly IUsuariosService _usuariosService = usuariosService;

        /// <summary>Usuarios list</summary>
        /// <remarks>It is possible return usuarios list.</remarks>
        [HttpGet, Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 10)]
        public async Task<ActionResult<UsuariosResponseDto>> GetUsuarios()
        {
            try
            {
                var response = await _usuariosService.SelectUsuariosService();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>Usuarios list</summary>
        /// <remarks>It is possible return usuarios list.</remarks>
        /// <param name="idUsuario" example="1">Parameters to get usuarios.</param>
        [HttpGet, Authorize]
        [Route("{idUsuario}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 10)]
        public async Task<ActionResult<UsuariosResponseDto>> GetUsuario(string idUsuario)
        {
            try
            {
                var response = await _usuariosService.SelectUsuariosPorIdService(idUsuario);
                if (response == null)
                    return NotFound();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>Usuarios login</summary>
        /// <remarks>It is possible return usuario login.</remarks>
        /// <param name="loginUsuario">Parameters to login usuario.</param>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<TokenResponseDto>> LoginUsuario([FromBody] LoginPayloadDto loginUsuario)
        {
            try
            {
                var response = await _usuariosService.LoginUsuarioService(loginUsuario);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>Refresh Token</summary>
        /// <remarks>It is possible user refresh token credentials.</remarks>
        /// <param name="actualToken">Token actual for refresh.</param>
        [HttpPut("refreshtoken"), Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(string actualToken)
        {
            try
            {
                var response = await _usuariosService.RefreshTokenService(actualToken);
                if (response == null)
                {
                    return Unauthorized(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>Usuarios creation</summary>
        /// <remarks>It is possible return usuario creation.</remarks>
        /// <param name="usuariosPayloadDto">Parameters to post usuario.</param>
        [HttpPost, Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<bool>> PostUsuarios([FromBody] UsuariosPayloadDto nuevoUsuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                nuevoUsuario.IdUsuarioIdentity = GetIdentitiesUser.GetCurrentUserId(HttpContext.User.Identities);
                var response = await _usuariosService.PostUsuariosService(nuevoUsuario);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>Usuarios update</summary>
        /// <remarks>It is possible return usuario update.</remarks>
        /// <param name="IdUsuario" example="1">Parameters to put usuario.</param>
        /// <param name="usuario">Parameters to put usuario.</param>
        [HttpPut, Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> PutUsuarios(string IdUsuario, UsuariosPayloadPutDto usuario)
        {
            try
            {
                usuario.IdUsuarioIdentity = GetIdentitiesUser.GetCurrentUserId(HttpContext.User.Identities);
                var response = await _usuariosService.PutUsuariosService(IdUsuario, usuario);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>Usuarios update</summary>
        /// <remarks>It is possible return usuario update.</remarks>
        /// <param name="idUsuario" example="1">Parameters to put usuario.</param>
        [HttpDelete, Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> DeleteUsuarios(string idUsuario)
        {
            try
            {
                var usuario = new UsuariosPayloadDeleteDto
                {
                    IdUsuarioIdentity = GetIdentitiesUser.GetCurrentUserId(HttpContext.User.Identities),
                    IdUsuario = idUsuario
                };
                var response = await _usuariosService.DeleteUsuariosService(usuario);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>Auditoria por usuario list</summary>
        /// <remarks>It is possible return auditorias list.</remarks>
        /// <param name="idUsuario" example="1">Parameters to get auditorias.</param>
        [HttpGet, Authorize]
        [Route("auditoria/{idUsuario}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 10)]
        public async Task<ActionResult<Auditoria>> GetAuditoria(string idUsuario)
        {
            try
            {
                var response = await _usuariosService.SelectUsuariosPorAuditoriaService(idUsuario);
                if (response == null)
                    return NotFound();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }