using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Services.Redis;
using psm_web_site_api_project.Services.Roles;

namespace psm_web_site_api_project.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class RolesController(IRolesService rolesService, IRedisService redisService) : ControllerBase
    {
        /// <summary>Roles list</summary>
        /// <remarks>It is possible return roles list.</remarks>
        [HttpGet, Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 10)]
        public async Task<ActionResult<Rol>> GetRoles()
        {
            try
            {
                string recordCacheKey = $"Roles_";
                var redisCacheResponse = await redisService.GetData<Rol>(recordCacheKey);
                if (redisCacheResponse != null && redisCacheResponse.Count > 0)
                {
                    return Ok(redisCacheResponse);
                }
                var rolesResponse = await rolesService.SelectRolesService();
                if(rolesResponse != null || rolesResponse?.Count > 0)
                    await redisService.SetData(recordCacheKey, rolesResponse);
                return Ok(rolesResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorHandler { Code = 400, Message = ex.Message });
            }
        }

        /// <summary>Roles single</summary>
        /// <remarks>It is possible return rol single.</remarks>
        /// <param name="idRol" example="1">Parameters to get rol.</param>
        [HttpGet, Authorize]
        [Route("{idRol}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 10)]
        public async Task<ActionResult<Rol>> GetRol(string idRol)
        {
            try
            {
                string recordCacheKey = $"Rol_{idRol}";
                var redisCacheResponse = await redisService.GetDataSingle<Rol>(recordCacheKey)!;
                if (redisCacheResponse != null)
                {
                    return Ok(redisCacheResponse);
                }
                var rolResponse = await rolesService.SelectRolPorIdService(idRol);
                if (rolResponse == null)
                    return NotFound(new ErrorHandler { Code = 404, Message = "Rol no encontrado" });
                await redisService.SetDataSingle(recordCacheKey, rolResponse);
                return Ok(rolResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorHandler { Code = 400, Message = ex.Message });
            }
        }
    }