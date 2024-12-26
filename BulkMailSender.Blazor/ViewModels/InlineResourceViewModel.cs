namespace BulkMailSender.Blazor.ViewModels {
    public class InlineResourceViewModel {
        public Guid Id { get; set; } // Unique identifier for the image
        public byte[] Content { get; set; } // Image content as Base64
        public string? FileName { get; set; } // Optional: Original file name
        public string? MimeType { get; set; }
    }
}
