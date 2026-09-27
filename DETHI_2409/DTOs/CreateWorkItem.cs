using DETHI_2409.Enums;
using System.ComponentModel.DataAnnotations;

namespace DETHI_2409.DTOs
{
    public class CreateWorkItem
    {

        [StringLength(200, MinimumLength = 5)]
        public string Title { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }
        public string Status { get; set; } = EnumName.Todo.ToString();
        public string ProjectCode { get; set; } // ORTHER
        public int? AssigneeId { get; set; }
        public string Priority { get; set; }
        public DateTime? DueAt { get; set; }
        public string[]? Labels { get; set; }
    }
}
