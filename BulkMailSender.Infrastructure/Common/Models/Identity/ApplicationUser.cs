using Microsoft.AspNetCore.Identity;

namespace BulkMailSender.Infrastructure.Common.Models.Identity {
    public class ApplicationUser : IdentityUser {    // Custom properties
        public string? DisplayName { get; set; }
        // public bool CanAccessEmailSending { get; set; } = false;

        public bool IsActive { get; set; } = true;
    }
}
