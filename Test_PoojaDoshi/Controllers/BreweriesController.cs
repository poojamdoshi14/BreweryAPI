using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Test_PoojaDoshi.Interfaces;
using Test_PoojaDoshi.Models;

namespace Test_PoojaDoshi.Controllers
{

    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BreweriesController : ControllerBase
    {
        private readonly IBreweryService _service;
        private readonly ILogger<BreweriesController> _logger;

        public BreweriesController(IBreweryService service, ILogger<BreweriesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Search breweries with optional sorting by name, city, or phone.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string? query,
            [FromQuery] string? city,
            [FromQuery] string? sortBy,
            [FromQuery] bool desc = false,
            [FromQuery] int? limit = 500,
            [FromQuery] int? offset = 0,
            CancellationToken ct = default)
        {
            try
            {
                _logger.LogInformation("Get All");

                var req = new BrewerySearchRequest
                {
                    Query = query,
                    City = city,
                    SortBy = sortBy,
                    Desc = desc,
                    Limit = limit,
                    Offset = offset
                };

                var result = await _service.SearchAsync(req, ct);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in BreweriesController.Get");
                return Problem("An unexpected error occurred.");
            }
        }
    }

}
