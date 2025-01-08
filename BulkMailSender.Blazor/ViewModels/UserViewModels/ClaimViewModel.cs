namespace BulkMailSender.Blazor.ViewModels.UserViewModels {
    public class ClaimViewModel {
        public string Type { get; set; }
        public string Value { get; set; }
        public string DisplayText => $"{Type}: {Value}";
    }
}
