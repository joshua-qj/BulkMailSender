namespace BulkMailSender.Application.Dtos {
    public class AttachmentDto {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public byte[] Content { get; set; }
    }
}
