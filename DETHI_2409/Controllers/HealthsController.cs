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
            var resError = new ResponseHealth<bool>();
            if (hc == false)
            {
                resError.Status = 503;
                resError.Message = "unavailable";
                return Conflict(resError);
            }

            resError.Status = 200;
            resError.Message = "available";
            return Ok(resError);
        }
    }
}
