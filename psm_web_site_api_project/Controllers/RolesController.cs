using Microsoft.AspNetCore.Mvc;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Services.Redis;
using psm_web_site_api_project.Services.Roles;

namespace psm_web_site_api_project.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class RolesController(IRolesService rolesService, IRedisService redisService) : ControllerBase
    {
        private readonly IRedisService _redisService = redisService;
        private readonly IRolesService _rolesService = rolesService;

        /// <summary>Roles list</summary>
        /// <remarks>It is possible return roles list.</remarks>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 10)]
        public async Task<ActionResult<Rol>> GetRoles()
        {
            try
            {
                string recordCacheKey = $"Roles{DateTime.Now:yyyyMMdd_hhmm}";
                var redisCacheResponse = await _redisService.GetData<Rol>(recordCacheKey);
                if (redisCacheResponse != null)
                {
                    return Ok(redisCacheResponse);
                }
                var rolesResponse = await _rolesService.SelectRolesService();
                await _redisService.SetData(recordCacheKey, rolesResponse);
                return Ok(rolesResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }