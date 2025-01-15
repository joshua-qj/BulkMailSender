using BulkMailSender.Domain.Entities.Email;
using BulkMailSender.Infrastructure.Common.Entities.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMailSender.Infrastructure.InMemoryPersistence.Contexts {
    public static class InMemoryDbContextExtensions {
        public static void Seed(this InMemoryDbContext context) {
            if (!context.Requesters.Any()) {
                context.Requesters.Add(new RequesterEntity {
                    Id = Guid.Parse("a6e9e69e-3af3-43c3-a6e9-775f751f3659"),
                    LoginName = "joshua.qj@hotmail.com",
                    Password = "YCNDXj6t7LfMc1yW",
                    MailServerId = Guid.Parse("a7dc4d29-69fa-4d8f-92aa-df9b3076aad1")
                });
                context.SaveChanges();
            }
            if (!context.MailServers.Any()) {
                context.MailServers.Add(new MailServerEntity {
                    Id = Guid.Parse("a7dc4d29-69fa-4d8f-92aa-df9b3076aad1"),
                    ServerName = "smtp-relay.brevo.com",
                    Port = 587,
                    IsSecure = true
                });
                context.SaveChanges();
            }
            if (!context.Statuses.Any()) {
                context.Statuses.AddRange(new StatusEntity { Id = 1, Name = "Ready" },
                new StatusEntity { Id = 2, Name = "Delivered" },
                new StatusEntity { Id = 3, Name = "Undelivered" },
                new StatusEntity { Id = 4, Name = "Retrying" },
                new StatusEntity { Id = 6, Name = "Canceled" },
                new StatusEntity { Id = 7, Name = "InvalidRecipient" }
                );
                context.SaveChanges();
            }
            /*            

            // Seeding Status
            modelBuilder.Entity<StatusEntity>().HasData(
                new StatusEntity { Id = 1, Name = "Ready" },
                new StatusEntity { Id = 2, Name = "Delivered" },
                new StatusEntity { Id = 3, Name = "Undelivered" },
                new StatusEntity { Id = 4, Name = "Retrying" },
                new StatusEntity { Id = 6, Name = "Canceled" },
                new StatusEntity { Id = 7, Name = "InvalidRecipient" }
            );


        }*/
            // Add similar checks and inserts for other entities as needed
        }
    }

}
