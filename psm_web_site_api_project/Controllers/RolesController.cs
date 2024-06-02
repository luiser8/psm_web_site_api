using Microsoft.AspNetCore.Mvc;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Services.Roles;

namespace psm_web_site_api_project.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class RolesController(IRolesService rolesService) : ControllerBase
    {
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
                var response = await _rolesService.SelectRolesService();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }