namespace DETHI_2409.Common
{
    public class ResponseSuccess<T>
    {
        public string? TraceId { get; set; }
        public int Status { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }

    public class ResponseError<T>
    {
        public string? TraceId { get; set; }
        public int Status { get; set; }
        public string? Message { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
    }

    public class ResponseHealth<T>
    {
        public int Status { get; set; }
        public string? Message { get; set; }
    }
}
