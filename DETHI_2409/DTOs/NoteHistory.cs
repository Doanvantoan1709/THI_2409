using System.ComponentModel.DataAnnotations;

namespace DETHI_2409.DTOs
{
    public class NoteHistory
    {
        [Required]
        [StringLength(1000, MinimumLength = 1)]
        public string note { get; set; }
    }
}
