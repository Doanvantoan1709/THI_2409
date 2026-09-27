using DETHI_2409.Common;
using DETHI_2409.Entities;
using DETHI_2409.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;

namespace DETHI_2409.Controllers
{
    [Route("api/health")]
    [ApiController]
    public class HealthsController : ControllerBase
    {
        private readonly IHealthService _healthService;

        public HealthsController(IHealthService healthService)
        {
            _healthService = healthService;
        }

        [HttpGet]
        public ActionResult<bool> HealthCheckDB()
        {
            var hc = _healthService.HealthCheckDB();
            if (hc == false)
            {
                return NotFound(new
                {
                    Status = 503,
                    Message = "unavailable"
                });
            }
            return Ok(new
            {
                Status = 200,
                Message = "Available"
            });
        }
    }
}
