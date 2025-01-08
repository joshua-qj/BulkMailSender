using BulkMailSender.Infrastructure.Common.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BulkMailSender.Infrastructure.SQLServerPersistence.Contexts {
    public class ApplicationUserDbContext : IdentityDbContext<ApplicationUser> {
        public ApplicationUserDbContext(DbContextOptions<ApplicationUserDbContext> options)
            : base(options) {
        }
    }
}
