namespace BulkMailSender.Blazor.ViewModels {
    public class StatusMessageViewModel {
        public string Message { get; set; } = string.Empty;
        public bool IsError { get; set; } = false;
    }
}
