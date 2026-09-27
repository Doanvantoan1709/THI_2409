namespace DETHI_2409.DTOs
{
    public class ReportProject
    {
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public long TotalItems { get; set; }
        public int OpenItems { get; set; }
        public int OverDueItems { get; set; }
        public int DoneItems { get; set; }
        public double?   AverageCompletionHours { get; set; }

    }
}
