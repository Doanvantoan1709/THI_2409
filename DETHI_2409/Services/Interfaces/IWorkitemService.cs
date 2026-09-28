
using DETHI_2409.DTOs;
using System.ComponentModel.DataAnnotations;

namespace DETHI_2409.Services.Interfaces
{
    public interface IWorkitemService
    {
        // PART 1
        Task<WorkItemCustom> GetWorkItemsAsync(FilterWorkItems filter, PagingWorkItems paging, SortWorkItems sort);

        Task<WorkItemDetailDto> GetWorkItemDetailAsync(int id);

        Task DeleteWorkItemAsync(int id);

        Task CreateWorkItemAsync(CreateWorkItem createWorkItem);

        Task AssignTasks(int id, ParameterItem parameterItem);



        // PART 2
        Task<HistoryDto[]> GetHistoryAsync(long id, DateTime? from, DateTime? to);
        Task<HistoryDto> WriteAndDisplayNoteAsync(long id, NoteHistory note);

    }
}
