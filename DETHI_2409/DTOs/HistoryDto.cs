using DETHI_2409.Entities;

namespace DETHI_2409.DTOs
{
    public class HistoryDto
    {
        public long Id { get; set; }

        public string? FromStatus { get; set; }

        public string? ToStatus { get; set; }

        public string? Note { get; set; }

        public string ChangedBy { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}
