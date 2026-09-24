using DETHI_2409.Common;
using DETHI_2409.DTOs;
using DETHI_2409.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;
using static System.Net.WebRequestMethods;

namespace DETHI_2409.Controllers
{
    [Route("api/work-items")]
    [ApiController]
    public class WorkItemsController : ControllerBase
    {
        private readonly IWorkitemService _workitemService;

        public WorkItemsController(IWorkitemService workitemService)
        {
            _workitemService = workitemService;
        }

        [HttpGet]
        public async Task<ActionResult> GetWorkItemsAsync(
            [FromQuery]FilterWorkItems filter, 
            [FromQuery]PagingWorkItems paging, 
            [FromQuery]SortWorkItems sort)
        {
            try
            {
                var getWordItems = await _workitemService.GetWorkItemsAsync(filter, paging, sort);

                var res = new ResponseSuccess<WorkItemCustom>
                {
                    TraceId = HttpContext.TraceIdentifier,
                    Status = 200,
                    Message = "Thanh cong",
                    Data = getWordItems
                };

                return Ok(res);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetWorkItemDetailAsync(int id)
        {
            try
            {
                var getWorkItemDetail = await _workitemService.GetWorkItemDetailAsync(id);

                var res = new ResponseSuccess<WorkItemDetailDto>
                {
                    TraceId = HttpContext.TraceIdentifier,
                    Status = 200,
                    Message = "Thanh cong",
                    Data = getWorkItemDetail
                };

                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task DeleteWorkItemAsync(int id)
        {
            try
            {
                await _workitemService.DeleteWorkItemAsync(id);
            }
            catch(Exception ex)
            {
                BadRequest(ex.Message);
            }
        }
    }
}
