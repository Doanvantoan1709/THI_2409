namespace DETHI_2409.DTOs
{
    public class WorkItems
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string ProjectCode { get; set; } // I JOIN
        public string ProjectName { get; set; } // I JOIN
        public DateTime? DueAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AssigneeName { get; set; } // L JOIN
        public string[] Labels { get; set; }
    }
}
