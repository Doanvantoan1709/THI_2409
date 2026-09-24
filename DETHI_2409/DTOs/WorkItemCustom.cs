using DETHI_2409.Entities;

namespace DETHI_2409.DTOs
{
    public class WorkItemCustom
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public List<WorkItems> Items { get; set; }
    }
}
