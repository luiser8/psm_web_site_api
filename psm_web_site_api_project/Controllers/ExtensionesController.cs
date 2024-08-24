using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using psm_web_site_api_project.Services.Redis;
using psm_web_site_api_project.Dto;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Services.Extensiones;
using psm_web_site_api_project.Utils.GetIdentities;

namespace psm_web_site_api_project.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class ExtensionesController(IExtensionesService extensionService, IRedisService redisService) : ControllerBase
    {
        private readonly IRedisService _redisService = redisService;
        private readonly IExtensionesService _extensionService = extensionService;

        /// <summary>Extensiones list</summary>
        /// <remarks>It is possible return extensiones list.</remarks>
        [HttpGet, Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 10)]
        public async Task<ActionResult<Extension>> GetExtensiones()
        {
            try
            {
                string recordCacheKey = $"Extensiones_";
                var redisCacheResponse = await _redisService.GetData<Extension>(recordCacheKey);
                if (redisCacheResponse != null && redisCacheResponse.Count > 0)
                {
                    return Ok(redisCacheResponse);
                }
                var extensionesResponse = await _extensionService.SelectExtensionesService();
                await _redisService.SetData(recordCacheKey, extensionesResponse);
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
                var response = await _extensionService.PostExtensionesService(extension);
                return Ok(response);
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
                var response = await _extensionService.PutExtensionesService(idExtension, extension);
                return Ok(response);
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
                var response = await _extensionService.DeleteExtensionesService(extensionEdit);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorHandler { Code = 400, Message = ex.Message });
            }
        }
    }