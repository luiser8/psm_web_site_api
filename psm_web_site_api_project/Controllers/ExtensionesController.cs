using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using psm_web_site_api_project.Dto;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Services.Extensiones;
using psm_web_site_api_project.Utils.GetIdentities;
using psm_web_site_api_project.Services.StatusResponse;

namespace psm_web_site_api_project.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class ExtensionesController(IExtensionesService extensionService) : ControllerBase
    {
        /// <summary>Extensiones list</summary>
        /// <remarks>It is possible return extensiones list.</remarks>
        [HttpGet, Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 10)]
        public async Task<ActionResult<Extension>> GetExtensiones()
        {
            try
            {
                var extensionesResponse = await extensionService.SelectExtensionesService();
                if (extensionesResponse?.Count == 0)
                    return NotFound(new ErrorHandler { Code = 404, Message = "No hay extensiones que mostrar" });
                return Ok(extensionesResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorHandler { Code = 400, Message = ex.Message });
            }
        }

        /// <summary>Extensiones creation</summary>
        /// <remarks>It is possible extensiones creation.</remarks>
        [HttpPost, Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 10)]
        public async Task<ActionResult<Extension>> PostExtensiones(ExtensionDto extension)
        {
            try
            {
                extension.IdUsuarioIdentity = GetIdentitiesUser.GetCurrentUserId(HttpContext.User.Identities);
                var response = await extensionService.PostExtensionesService(extension);
                return Ok(GetStatusResponse.GetStatusResponses(response, "Extension", "guardada"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorHandler { Code = 400, Message = ex.Message });
            }
        }

        /// <summary>Extensiones edit</summary>
        /// <remarks>It is possible extensiones edit.</remarks>
        [HttpPut, Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 10)]
        public async Task<ActionResult<Extension>> PutExtensiones(string idExtension, ExtensionDto extension)
        {
            try
            {
                extension.IdUsuarioIdentity = GetIdentitiesUser.GetCurrentUserId(HttpContext.User.Identities);
                var response = await extensionService.PutExtensionesService(idExtension, extension);
                return Ok(GetStatusResponse.GetStatusResponses(response, "Extension", "actualizada"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorHandler { Code = 400, Message = ex.Message });
            }
        }

        /// <summary>Extensiones delete</summary>
        /// <remarks>It is possible delete edit.</remarks>
        [HttpDelete, Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 10)]
        public async Task<ActionResult<Extension>> DeleteExtensiones(string idExtension)
        {
            try
            {
                var extensionEdit = new ExtensionDto
                {
                    IdUsuarioIdentity = GetIdentitiesUser.GetCurrentUserId(HttpContext.User.Identities),
                    IdExtension = idExtension
                };
                var response = await extensionService.DeleteExtensionesService(extensionEdit);
                return Ok(GetStatusResponse.GetStatusResponses(response, "Extension", "eliminada"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorHandler { Code = 400, Message = ex.Message });
            }
        }
    }