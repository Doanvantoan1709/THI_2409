
using DETHI_2409.DTOs;

namespace DETHI_2409.Services.Interfaces
{
    public interface IWorkitemService
    {
        Task<WorkItemCustom> GetWorkItemsAsync(FilterWorkItems filter, PagingWorkItems paging, SortWorkItems sort);

        Task<WorkItemDetailDto> GetWorkItemDetailAsync(int id);

        Task DeleteWorkItemAsync(int id);

        Task CreateWorkItemAsync(CreateWorkItem createWorkItem);
    }
}
