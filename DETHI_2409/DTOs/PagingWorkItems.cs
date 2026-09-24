using System.ComponentModel.DataAnnotations;

namespace DETHI_2409.DTOs
{
    public class PagingWorkItems
    {
        public int Page { get; set; } = 1;

        //[StringLength(50, MinimumLength = 1)]
        [Range(1, 50)]
        public int PageSize { get; set; } = 20;
    }
}
