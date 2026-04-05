namespace Library.MVC.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }

    public class StatusCodeViewModel
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
    }
}