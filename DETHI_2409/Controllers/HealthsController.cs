using DETHI_2409.Common;
using DETHI_2409.DTOs;
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
        public async Task<ActionResult> HealthCheckDB()
        {
            try
            {
                var data = await _healthService.HealthCheckDB();
                if(data.DbConnected == true)
                {
                    var res = new Response<HealthDto>
                    {
                        TraceId = HttpContext.TraceIdentifier,
                        Status = 200,
                        Message = "Thành công",
                        Data = data
                    };
                    return Ok(res);
                }

                var resErr = new Response<HealthDto>
                {
                    TraceId = HttpContext.TraceIdentifier,
                    Status = 503,
                    Message = "Thất bại",
                    Data = data
                };
                return Ok(resErr);

            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
