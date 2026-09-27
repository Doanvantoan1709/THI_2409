namespace DETHI_2409.DTOs
{
    public class WorkItemHistoryDetail
    {
        public string? FromStatus { get; set; }
        public string? ToStatus { get; set; }
        public string? Note { get; set; }
        public string? Changeby { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
