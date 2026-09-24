namespace DETHI_2409.DTOs
{
    public class WorkItemDetailDto
    {
        public WorkItemDetail? Item { get; set; }
        public ProjectDetail? Project { get; set; }
        public DeveloperDetail? Assignee { get; set; }
        public string[]? Labels { get; set; }
        public WorkItemHistoryDetail[]? History { get; set; }
    }
}
