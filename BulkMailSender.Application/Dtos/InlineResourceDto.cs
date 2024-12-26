namespace BulkMailSender.Application.Dtos {
    public class InlineResourceDto {
        public Guid Id { get; set; }
        public byte[] Content { get; set; }
        public string? FileName { get; set; } // Optional: The file name of the resource
        public string? MimeType { get; set; }
    }
}
