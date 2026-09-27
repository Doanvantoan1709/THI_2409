using System.ComponentModel.DataAnnotations;

namespace DETHI_2409.DTOs
{
    public class ParameterItem
    {
        public long? AssigneeId { get; set; }

        [StringLength(1000)]
        public string? Note { get; set; }
    }
}
