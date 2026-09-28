using DETHI_2409.Common;
using DETHI_2409.DTOs;
using DETHI_2409.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DETHI_2409.Controllers
{
    [Route("api/reports/project-summary")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ProjectsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        public async Task<IActionResult> GetReportProject([FromQuery]ParameterProject parameterProject)
        {
            try
            {
                var data = await _reportService.GetReportProject(parameterProject);

                var res = new ListResponseSuccess<ReportProject>
                {
                    TraceId = HttpContext.TraceIdentifier,
                    Status = 200,
                    Message = "Thành công",
                    Data = data
                };

                return Ok(res);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
