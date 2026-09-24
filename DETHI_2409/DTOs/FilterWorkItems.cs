using System.ComponentModel.DataAnnotations;

namespace DETHI_2409.DTOs
{
    public class FilterWorkItems
    {
        public string? Keyword { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public string? ProjectCode { get; set; } // JOIN
        [MinLength(1)]
        public int? AssigneeId { get; set; }
        public bool? Overdue { get; set; } = false;
    }
}
