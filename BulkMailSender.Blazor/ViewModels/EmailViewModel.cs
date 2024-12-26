using System.ComponentModel.DataAnnotations;

namespace BulkMailSender.Blazor.ViewModels {
    public class EmailViewModel {
        [Required(ErrorMessage = "Email From is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address format")]
        public string EmailFrom { get; set; }=null!;

        public string? DisplayName { get; set; }

        public string EmailTo { get; set; } = null!;

        [Required(ErrorMessage = "Email Subject is required")]
        [MinLength(1, ErrorMessage = "Email Subject must be at least 10 characters long viewmodel")]
        public string Subject { get; set; } = null!;

        [Required(ErrorMessage = "Email Body is required")]
        [MinLength(1, ErrorMessage = "Email Body must be at least 1 characters long viewmodel")]
        public string Body { get; set; } = null!; //"<p>Dear {name},</p> <p>Enter your email content here...</p>";

        //        public Guid RequesterID { get; set; }
        //public Requester Requester { get; set; }

        // Email Address in lowercase as specified in the query
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string TestRecipientEmail { get; set; }
        // List of attachments
        public List<AttachmentViewModel> Attachments { get; set; } = new List<AttachmentViewModel>();

        public List<InlineResourceViewModel> InlineResources { get; set; } = new List<InlineResourceViewModel>();
    }
}
