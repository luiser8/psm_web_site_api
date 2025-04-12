using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Payloads;
using psm_web_site_api_project.Responses;
using psm_web_site_api_project.Services.Carousel;
using psm_web_site_api_project.Services.Redis;
using psm_web_site_api_project.Services.StatusResponse;
using psm_web_site_api_project.Utils.GetIdentities;

namespace psm_web_site_api_project.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class CarouselController(ICarouselService carouselService, IRedisService redisService) : ControllerBase
    {
        /// <summary>Carousel list</summary>
        /// <remarks>It is possible return carousel list for extension.</remarks>
        /// <param name="idExtension" example="1">Parameters to get idExtension.</param>
        [HttpGet, Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 10)]
        public async Task<ActionResult<CarouselResponse>> GetCarousel([FromQuery] string? idExtension)
        {
            try
            {
                var recordCacheKey = $"Carousel_{idExtension}";
                var redisCacheResponse = await redisService.GetDataSingle<Carousel>(recordCacheKey)!;
                if (redisCacheResponse != null)
                {
                    return Ok(redisCacheResponse);
                }
                var carouselResponse = await carouselService.SelectCarouselPorIdExtensionService(idExtension);
                if (carouselResponse.IdCarousel == null)
                    return NotFound(new ErrorHandler { Code = 404, Message = "Carousel no encontrado" });
                await redisService.SetDataSingle(recordCacheKey, carouselResponse);
                return Ok(carouselResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorHandler { Code = 400, Message = ex.Message });
            }
        }

        /// <summary>
        /// Creates a new Carousel
        /// </summary>
        [HttpPost, Authorize]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Carousel>> PostHeader([FromForm] CarouselPayload carouselPayload)
        {
            try
            {
                carouselPayload.IdUsuarioIdentity = GetIdentitiesUser.GetCurrentUserId(HttpContext.User.Identities);
                var response = await carouselService.PostCarouselService(carouselPayload);
                return Ok(GetStatusResponse.GetStatusResponses(response, "Carousel", "guardado"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorHandler { Code = 400, Message = ex.Message });
            }
        }

        /// <summary>Carousel edit</summary>
        /// <remarks>It is possible carousel edit.</remarks>
        [HttpPut, Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 10)]
        public async Task<ActionResult<Carousel>> PutCarousel(string idCarousel, [FromForm] CarouselPayload carouselPayload)
        {
            try
            {
                carouselPayload.IdUsuarioIdentity = GetIdentitiesUser.GetCurrentUserId(HttpContext.User.Identities);
                var response = await carouselService.PutCarouselService(idCarousel, carouselPayload);
                return Ok(GetStatusResponse.GetStatusResponses(response, "Carousel", "actualizado"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorHandler { Code = 400, Message = ex.Message });
            }
        }

        /// <summary>Carousel delete</summary>
        /// <remarks>It is possible return carousel delete.</remarks>
        /// <param name="idCarousel" example="1">Parameters to delete carousel.</param>
        [HttpDelete, Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> DeleteCarousel(string idCarousel)
        {
            try
            {
                var carousel = new CarouselPayload
                {
                    IdUsuarioIdentity = GetIdentitiesUser.GetCurrentUserId(HttpContext.User.Identities),
                    IdCarousel = idCarousel
                };
                var response = await carouselService.DeleteCarouselService(carousel);
                return Ok(GetStatusResponse.GetStatusResponses(response, "Carousel", "eliminado"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorHandler { Code = 400, Message = ex.Message });
            }
        }

        /// <summary>Carousel add or remove items</summary>
        /// <remarks>It is possible to add or remove items to carousels.</remarks>
        [HttpPatch, Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 10)]
        public async Task<ActionResult<Carousel>> PatchCarousels(string idExtension, string type, string? itemToRemove, CarouselCollectionPayload carousel)
        {
            try
            {
                var response = type == "add"
                                ? await carouselService.AddItemToCarousel(idExtension, carousel)
                                : type == "remove" && itemToRemove != null
                                    ? await carouselService.RemoveItemFromCarousel(idExtension, itemToRemove)
                                    : throw new ArgumentNullException(nameof(itemToRemove), "Item to remove cannot be null");
                return Ok(GetStatusResponse.GetStatusResponses(response, "Carousel", type == "add" ? "item agregado" : "item eliminado"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorHandler { Code = 400, Message = ex.Message });
            }
        }
    }