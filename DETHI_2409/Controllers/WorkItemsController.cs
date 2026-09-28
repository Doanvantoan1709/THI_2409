using DETHI_2409.Common;
using DETHI_2409.DTOs;
using DETHI_2409.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.NetworkInformation;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            [FromQuery] FilterWorkItems filter,
            [FromQuery] PagingWorkItems paging,
            [FromQuery] SortWorkItems sort)
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
            catch (Exception ex)
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

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteWorkItemAsync(int id)
        {
            try
            {
                await _workitemService.DeleteWorkItemAsync(id);

                return Ok(new
                {
                    message = "Xoa thanh cong"
                });
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateWorkItemAsync(CreateWorkItem createWorkItem)
        {
            try
            {
                await _workitemService.CreateWorkItemAsync(createWorkItem);

                return Ok(new
                {
                    message = "Tao thanh cong"
                });

            } catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPatch("{id}/assignee")]
        public async Task<ActionResult> AssignTasks(int id, ParameterItem parameterItem)
        {
            try
            {
                await _workitemService.AssignTasks(id, parameterItem);
                return Ok(new
                {
                    message = "Phân công hoàn tất"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("{id}/history")]
        public async Task<ActionResult> GetHistoryAsync(long id, DateTime? from, DateTime? to)
        {
            try
            {
                var data = await _workitemService.GetHistoryAsync(id, from, to);

                var res = new ArrResponseSuccess<HistoryDto>
                {
                    TraceId = HttpContext.TraceIdentifier,
                    Status = 200,
                    Message = "Thanh cong",
                    Data = data
                };

                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/notes")]
        public async Task<ActionResult> WriteAndDisplayNoteAsync(long id, [FromBody]NoteHistory note)
        {
            try
            {
                var data = await _workitemService.WriteAndDisplayNoteAsync(id, note);

                var res = new ResponseSuccess<HistoryDto>
                {
                    TraceId = HttpContext.TraceIdentifier,
                    Status = 200,
                    Message = "Thanh cong",
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
