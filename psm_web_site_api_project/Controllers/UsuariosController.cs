using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Payloads;
using psm_web_site_api_project.Responses;
using psm_web_site_api_project.Services.Redis;
using psm_web_site_api_project.Services.StatusResponse;
using psm_web_site_api_project.Services.Usuarios;
using psm_web_site_api_project.Utils.GetIdentities;

namespace psm_web_site_api_project.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController(IUsuariosService usuariosService, IRedisService redisService) : ControllerBase
    {
        /// <summary>Usuarios list</summary>
        /// <remarks>It is possible return usuarios list.</remarks>
        [HttpGet, Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 10)]
        public async Task<ActionResult<UsuariosResponse>> GetUsuarios()
        {
            try
            {
                const string recordCacheKey = $"Usuarios_";
                var redisCacheResponse = await redisService.GetData<UsuariosResponse>(recordCacheKey);
                if (redisCacheResponse.Count > 0)
                {
                    return Ok(redisCacheResponse);
                }
                var usuariosResponse = await usuariosService.SelectUsuariosService();
                if(usuariosResponse?.Count > 0)
                    await redisService.SetData(recordCacheKey, usuariosResponse);
                return Ok(usuariosResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorHandler { Code = 400, Message = ex.Message });
            }
        }

        /// <summary>Usuarios list</summary>
        /// <remarks>It is possible return usuarios list.</remarks>
        /// <param name="idUsuario" example="1">Parameters to get usuarios.</param>
        [HttpGet, Authorize]
        [Route("{idUsuario}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 10)]
        public async Task<ActionResult<UsuariosResponse>> GetUsuario(string idUsuario)
        {
            try
            {
                var usuarioResponse = await usuariosService.SelectUsuariosPorIdService(idUsuario);
                return Ok(usuarioResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorHandler { Code = 400, Message = ex.Message });
            }
        }

        /// <summary>Usuarios creation</summary>
        /// <remarks>It is possible return usuario creation.</remarks>
        /// <param name="nuevoUsuario">Parameters to post usuario.</param>
        [HttpPost, Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<bool>> PostUsuarios([FromBody] UsuarioPayload nuevoUsuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                nuevoUsuario.IdUsuarioIdentity = GetIdentitiesUser.GetCurrentUserId(HttpContext.User.Identities);
                var response = await usuariosService.PostUsuariosService(nuevoUsuario);
                return Ok(GetStatusResponse.GetStatusResponses(response, "Usuario", "guardado"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorHandler { Code = 400, Message = ex.Message });
            }
        }

        /// <summary>Usuarios update</summary>
        /// <remarks>It is possible return usuario update.</remarks>
        /// <param name="idUsuario" example="1">Parameters to put usuario.</param>
        /// <param name="usuario">Parameters to put usuario.</param>
        [HttpPut, Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> PutUsuarios(string idUsuario, UsuariosPayloadPut usuario)
        {
            try
            {
                usuario.IdUsuarioIdentity = GetIdentitiesUser.GetCurrentUserId(HttpContext.User.Identities);
                var response = await usuariosService.PutUsuariosService(idUsuario, usuario);
                return Ok(GetStatusResponse.GetStatusResponses(response, "Usuario", "actualizado"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorHandler { Code = 400, Message = ex.Message });
            }
        }

        /// <summary>Usuarios update</summary>
        /// <remarks>It is possible return usuario update.</remarks>
        /// <param name="idUsuario" example="1">Parameters to put usuario.</param>
        /// <param name="status" example="true">Parameters to put usuario.</param>
        [HttpPatch, Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> SetStatusUsuarios(string idUsuario, bool status)
        {
            try
            {
                var usuario = new UsuariosPayloadDelete
                {
                    IdUsuarioIdentity = GetIdentitiesUser.GetCurrentUserId(HttpContext.User.Identities),
                    IdUsuario = idUsuario
                };
                var response = await usuariosService.SetStatusUsuariosService(usuario, status);
                return Ok(GetStatusResponse.GetStatusResponses(response, "Usuario", "estatus actualizado"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorHandler { Code = 400, Message = ex.Message });
            }
        }

        /// <summary>Usuarios delete</summary>
        /// <remarks>It is possible return usuario delete.</remarks>
        /// <param name="idUsuario" example="1">Parameters to delete usuario.</param>
        [HttpDelete, Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> DeleteUsuarios(string idUsuario)
        {
            try
            {
                var usuario = new UsuariosPayloadDelete
                {
                    IdUsuarioIdentity = GetIdentitiesUser.GetCurrentUserId(HttpContext.User.Identities),
                    IdUsuario = idUsuario
                };
                var response = await usuariosService.DeleteUsuariosService(usuario);
                return Ok(GetStatusResponse.GetStatusResponses(response, "Usuario", "eliminado"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorHandler { Code = 400, Message = ex.Message });
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
                var response = await usuariosService.SelectUsuariosPorAuditoriaService(idUsuario);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorHandler { Code = 400, Message = ex.Message });
            }
        }
    }