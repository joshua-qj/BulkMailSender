using System.ComponentModel.DataAnnotations;

namespace BulkMailSender.Blazor.ViewModels {
    public class EmailViewModel {
        [Required(ErrorMessage = "Email From is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address format")]
        public string EmailFrom { get; set; } = null!;

        public string? DisplayName { get; set; }

        public string EmailTo { get; set; } = null!;

        [Required(ErrorMessage = "Email Subject is required")]
        [MinLength(1, ErrorMessage = "Email Subject must be at least 10 characters long viewmodel")]
        public string Subject { get; set; } = null!;

        [Required(ErrorMessage = "Email Body is required")]
        [MinLength(1, ErrorMessage = "Email Body must be at least 1 characters long viewmodel")]
        public string Body { get; set; } = null!;
        public bool IsBodyHtml { get; set; }
        public Guid RequesterID { get; set; }
        public DateTime RequestedDate { get; set; }
        public RequesterViewModel Requester { get; set; } //maybe this should be removed

        // Email Address in lowercase as specified in the query
        [EmailAddress(ErrorMessage = "Please enter a valid test recipient email address.")]
        public string? TestRecipientEmail { get; set; }
        public Guid? BatchID { get; set; }
        public Guid UserId { get; set; }
        // List of attachments
        public HashSet<AttachmentViewModel> Attachments { get; set; } = new HashSet<AttachmentViewModel>();

        public List<InlineResourceViewModel> InlineResources { get; set; } = new List<InlineResourceViewModel>();
    }
}
