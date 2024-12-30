
namespace BulkMailSender.Application.Dtos {
    public class EmailDto {
        public Guid Id { get; set; }
        public string EmailFrom { get; set; }
        public string DisplayName { get; set; }
        public string EmailTo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool? IsBodyHtml { get; set; }
        public Guid RequesterID { get; set; }
        public RequesterDto Requester { get; set; }
        public DateTime RequestedDate { get; set; }
        public int StatusId { get; set; } // Maps to the enum
        public string? ErrorMessage { get; set; }
        public Guid? BatchID { get; set; }
        public Guid UserId { get; set; }
        public ICollection<AttachmentDto> Attachments { get; set; } = new List<AttachmentDto>();
        public ICollection<InlineResourceDto> InlineResources { get; set; } = new List<InlineResourceDto>();
    }
}
