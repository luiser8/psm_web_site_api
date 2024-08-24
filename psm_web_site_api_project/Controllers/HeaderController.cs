using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Repository.Headers;
using psm_web_site_api_project.Services.Redis;
using psm_web_site_api_project.Services.StatusResponse;
using psm_web_site_api_project.Utils.GetIdentities;

namespace psm_web_site_api_project.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class HeaderController(IHeaderService headerService, IRedisService redisService) : ControllerBase
    {
        private readonly IRedisService _redisService = redisService;
        private readonly IHeaderService _headerService = headerService;

        /// <summary>Header creation</summary>
        /// <remarks>It is possible header post for extensions.</remarks>
        [HttpPost, Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 10)]
        public async Task<ActionResult<Header>> PostHeader(HeaderDto header)
        {
            try
            {
                header.IdUsuarioIdentity = GetIdentitiesUser.GetCurrentUserId(HttpContext.User.Identities);
                var response = await _headerService.PostHeaderService(header);
                return Ok(GetStatusResponse.GetStatusResponses(response, "Header", "guardado"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorHandler { Code = 400, Message = ex.Message });
            }
        }
    }