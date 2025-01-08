using Microsoft.AspNetCore.Identity;

namespace BulkMailSender.Infrastructure.Common.Entities.Identity {
    public class ApplicationUser : IdentityUser {    // Custom properties
        public string? DisplayName { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
