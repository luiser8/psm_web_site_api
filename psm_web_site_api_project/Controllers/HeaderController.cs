using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Payloads;
using psm_web_site_api_project.Responses;
using psm_web_site_api_project.Services.Headers;
using psm_web_site_api_project.Services.Redis;
using psm_web_site_api_project.Services.StatusResponse;
using psm_web_site_api_project.Utils.GetIdentities;

namespace psm_web_site_api_project.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class HeaderController(IHeaderService headerService, IRedisService redisService) : ControllerBase
    {
        /// <summary>Header list</summary>
        /// <remarks>It is possible return header list for extension.</remarks>
        /// <param name="idExtension" example="1">Parameters to get idExtension.</param>
        [HttpGet, Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 10)]
        public async Task<ActionResult<HeaderResponse>> GetHeader([FromQuery] string? idExtension)
        {
            try
            {
                var recordCacheKey = $"Header_{idExtension}";
                var redisCacheResponse = await redisService.GetDataSingle<Header>(recordCacheKey)!;
                if (redisCacheResponse != null)
                {
                    return Ok(redisCacheResponse);
                }
                var headerResponse = await headerService.SelectHeaderPorIdExtensionService(idExtension);
                if (headerResponse.IdHeader == null)
                    return NotFound(new ErrorHandler { Code = 404, Message = "Header no encontrado" });
                await redisService.SetDataSingle(recordCacheKey, headerResponse);
                return Ok(headerResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorHandler { Code = 400, Message = ex.Message });
            }
        }

        /// <summary>
        /// Creates a new Header
        /// </summary>
        [HttpPost, Authorize]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Header>> PostHeader([FromForm] HeaderPayload header)
        {
            try
            {
                header.IdUsuarioIdentity = GetIdentitiesUser.GetCurrentUserId(HttpContext.User.Identities);
                var response = await headerService.PostHeaderService(header);
                return Ok(GetStatusResponse.GetStatusResponses(response, "Header", "guardado"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorHandler { Code = 400, Message = ex.Message });
            }
        }

        /// <summary>Headers edit</summary>
        /// <remarks>It is possible headers edit.</remarks>
        [HttpPut, Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 10)]
        public async Task<ActionResult<bool>> PutHeaders(string idHeader, [FromForm] HeaderPayload headerDto)
        {
            try
            {
                headerDto.IdUsuarioIdentity = GetIdentitiesUser.GetCurrentUserId(HttpContext.User.Identities);
                var response = await headerService.PutHeaderService(idHeader, headerDto);
                return Ok(GetStatusResponse.GetStatusResponses(response, "Header", "actualizado"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorHandler { Code = 400, Message = ex.Message });
            }
        }

        /// <summary>Headers delete</summary>
        /// <remarks>It is possible return header delete.</remarks>
        /// <param name="idHeader" example="1">Parameters to delete header.</param>
        [HttpDelete, Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> DeleteHeaders(string idHeader)
        {
            try
            {
                var header = new HeaderPayload
                {
                    IdUsuarioIdentity = GetIdentitiesUser.GetCurrentUserId(HttpContext.User.Identities),
                    IdHeader = idHeader
                };
                var response = await headerService.DeleteHeaderService(header);
                return Ok(GetStatusResponse.GetStatusResponses(response, "Header", "eliminado"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorHandler { Code = 400, Message = ex.Message });
            }
        }

        /// <summary>Headers add or remove items</summary>
        /// <remarks>It is possible to add or remove items to headers.</remarks>
        [HttpPatch, Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 10)]
        public async Task<ActionResult<bool>> PatchHeaders(string idExtension, string type, string? itemToRemove, HeaderCollection headerDto)
        {
            try
            {
                var response = type == "add"
                                ? await headerService.AddItemToHeader(idExtension, headerDto)
                                : type == "remove" && itemToRemove != null
                                    ? await headerService.RemoveItemFromHeader(idExtension, itemToRemove)
                                    : throw new ArgumentNullException(nameof(itemToRemove), "Item to remove cannot be null");
                return Ok(GetStatusResponse.GetStatusResponses(response, "Header", type == "add" ? "item agregado" : "item eliminado"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorHandler { Code = 400, Message = ex.Message });
            }
        }
    }