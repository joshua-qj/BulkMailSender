using BulkMailSender.Infrastructure.Common.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BulkMailSender.Infrastructure.InMemoryPersistence.Contexts {
    public class InMemoryIdentityDbContext : IdentityDbContext<ApplicationUser> {
        public InMemoryIdentityDbContext(DbContextOptions<InMemoryIdentityDbContext> options)
            : base(options) {
        }
    }
}
