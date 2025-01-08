using AutoMapper;
using BulkMailSender.Application.Dtos;
using BulkMailSender.Application.Interfaces.Email;
using BulkMailSender.Domain.Entities.Email;
using BulkMailSender.Infrastructure.Common.Entities.Email;
using BulkMailSender.Infrastructure.SQLServerPersistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace BulkMailSender.Infrastructure.SQLServerPersistence.Repositories {
    public class SqlServerEmailRepository : IEmailRepository {
        private readonly IDbContextFactory<SqlServerDbContext> _contextFactory;
        private readonly SqlServerDbContext _dbContext;
        private readonly IMapper _mapper;
        private Dictionary<Guid, Requester> _requesterConfigurations;
        public SqlServerEmailRepository(IDbContextFactory<SqlServerDbContext> contextFactory,
            SqlServerDbContext dbContext,

            IMapper mapper) {
            _contextFactory = contextFactory;
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper;
            _requesterConfigurations = new Dictionary<Guid, Requester>();
        }
        public Task<Attachment> FindOrAddAttachmentAsync(Attachment attachment) {
            throw new NotImplementedException();
        }

        public Task<InlineResource> FindOrAddInlineResourceAsync(InlineResource inlineResource) {
            throw new NotImplementedException();
        }

        public IQueryable<JobSummary> GetGroupedEmails(Guid userId) {
            throw new NotImplementedException();
        }

        public async Task<Requester> GetRequesterByIdAsync(Guid requesterId) {
            if (!_requesterConfigurations.Any()) {
                await LoadRequesterConfigurationsAsync();
            }

            _requesterConfigurations.TryGetValue(requesterId, out var requester);
            return await Task.FromResult(requester);
        }

        public async Task<Requester> GetRequesterByNameAsync(string hostName) {
            using var dbContext = _contextFactory.CreateDbContext();

            var requesterEntity = await dbContext.Requesters
                        .Include(r => r.MailServer) // Include related MailServer if needed
                                .AsNoTracking()
                                .FirstOrDefaultAsync(r => r.LoginName == hostName);

            // Map to domain model and return
            return _mapper.Map<Requester?>(requesterEntity);
        }

        public async Task LoadRequesterConfigurationsAsync() {
            using var dbContext = _contextFactory.CreateDbContext();

            var requesterEntities = await dbContext.Requesters
                .AsNoTracking() // No tracking for better performance
                .Include(r => r.MailServer) // Include Host for related data
                .ToListAsync();
            // Initialize the dictionary and map each RequesterEntity to Requester
            _requesterConfigurations = requesterEntities
                .ToDictionary(
                    requesterEntity => requesterEntity.Id,
                    requesterEntity => _mapper.Map<Requester>(requesterEntity)
                );
        }

        public async Task<Email> SaveEmailAsync(Email email) {
            using var dbContext = _contextFactory.CreateDbContext();
            if (email == null) {
                throw new ArgumentNullException(nameof(email));
            } else {
                var emailEntity = _mapper.Map<EmailEntity>(email);
                //mapping isue with attachment
                if (emailEntity != null) {

                    // Check and save attachments
                    foreach (var attachment in email.Attachments) {
                        var existingAttachment = await dbContext.Attachments
                            .FirstOrDefaultAsync(a => a.Id== attachment.Id);

                        //AttachmentEntity attachmentEntity;
                        if (existingAttachment == null) {
                            var attachmentEntity = _mapper.Map<AttachmentEntity>(attachment);
                            dbContext.Attachments.Add(attachmentEntity);
                        }
                        emailEntity.EmailAttachments.Add(new EmailAttachmentEntity {
                            EmailId = emailEntity.Id,
                            AttachmentId = attachment.Id
                        });
                    }
                    foreach (var inlineResource in email.InlineResources) {
                        var existingInlineResource = await dbContext.InlineResources
                            .FirstOrDefaultAsync(a => a.Id == inlineResource.Id);

                        //AttachmentEntity attachmentEntity;
                        if (existingInlineResource == null) {
                            var inlineResourceEntity = _mapper.Map<InlineResourceEntity>(inlineResource);
                            dbContext.InlineResources.Add(inlineResourceEntity);
                        }
                        emailEntity.EmailInlineResources.Add(new EmailInlineResourceEntity {
                            EmailId = emailEntity.Id,
                            InlineResourceId = inlineResource.Id
                        });
                    }
                    await dbContext.Emails.AddAsync(emailEntity);
                    if (emailEntity.Status != null) {
                        dbContext.Entry(emailEntity.Status).State = EntityState.Unchanged;
                    }
                    if (emailEntity.Requester != null) {
                        dbContext.Entry(emailEntity.Requester).State = EntityState.Unchanged;
                    }
                    await dbContext.SaveChangesAsync();
                    email.Id = emailEntity.Id; // Assuming the Id is generated after SaveChangesAsync

                }
            }
            return email;

        }

        public async Task UpdateEmailStatusAsync(Email email, string? errorMessage) {
            using var dbContext = _contextFactory.CreateDbContext();

            var emailEntity = await dbContext.Emails.FindAsync(email.Id);
            if (emailEntity == null) {
                throw new KeyNotFoundException($"Email not found, can not update email status.");
            }
            emailEntity.StatusId = email.StatusId;
            emailEntity.ErrorMessage = email.ErrorMessage;
            dbContext.Emails.Update(emailEntity);
            await dbContext.SaveChangesAsync();
        }
    }
}
