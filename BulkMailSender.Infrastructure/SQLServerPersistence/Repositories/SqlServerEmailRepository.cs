using AutoMapper;
using BulkMailSender.Application.Dtos;
using BulkMailSender.Application.Interfaces.Email;
using BulkMailSender.Domain.Entities.Email;
using BulkMailSender.Infrastructure.Common.Entities.Email;
using BulkMailSender.Infrastructure.SQLServerPersistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Net.Mail;

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

        public async Task<Email?> GetEmailByIdAsync(Guid emailId) {
            if (emailId == Guid.Empty)
                return null;
            using var dbContext = _contextFactory.CreateDbContext();

            var emailEntity = await dbContext
                .Emails.AsNoTracking()
                .Include(email => email.EmailAttachments)
                    .ThenInclude(emailAttachment => emailAttachment.Attachment)
                .Include(email => email.EmailInlineResources)
                    .ThenInclude(emaiInlineResources => emaiInlineResources.InlineResource)
                .FirstOrDefaultAsync(email => email.Id == emailId);

            try {
                return _mapper.Map<Email>(emailEntity);
            }
            catch (Exception ex) {

                throw ex;
            }

        }

        public IQueryable<JobSummaryDto> GetJobSummaries(Guid userId) {
            return _dbContext.Emails
    .Where(email => email.UserId == userId && email.BatchId != null)
    .GroupBy(email => email.BatchId)
    .Select(group => new JobSummaryDto {
        EmailId = group.First().Id,
        BatchId = group.Key,
        Date = group.Min(email => email.RequestedDate) ?? DateTime.MinValue,
        EmailFrom = group.First().EmailFrom,
        Subject = group.First().Subject,
        TotalEmailsSent = group.Count(),
        SuccessfulEmails = group.Count(email => email.StatusId == 2),
        FailedEmails = group.Count(email => email.StatusId == 3),
        PendingEmails = group.Count(email => email.StatusId == 1)
    });

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

        private readonly ConcurrentDictionary<Guid, object> _attachmentLocks = new();
        private readonly ConcurrentDictionary<Guid, object> _inlineResourceLocks = new();
        public async Task<Email> SaveEmailAsync(Email email) {
            using var dbContext = _contextFactory.CreateDbContext();
            if (email == null) return null;

            var emailEntity = _mapper.Map<EmailEntity>(email);
            if (emailEntity == null) return null;

            // Handle attachments
            foreach (var attachment in email.Attachments) {
                // Ensure lock exists for this attachment ID
                var lockObject = _attachmentLocks.GetOrAdd(attachment.Id, _ => {
                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}: attachment.Id = {attachment.Id} start lock");
                    return new object();
                });
                Console.WriteLine($"lockObject hashcode {lockObject.GetHashCode().ToString()}: attachment.Id = {attachment.Id} start lock");

                // Use the lock object to synchronize
                lock (lockObject) {
                    var existingAttachment = dbContext.Attachments
                        .FirstOrDefault(a => a.Id == attachment.Id);

                    if (existingAttachment == null) {
                        var attachmentEntity = _mapper.Map<AttachmentEntity>(attachment);
                        dbContext.Attachments.Add(attachmentEntity);
                        dbContext.SaveChanges();
                    }
                }
                emailEntity.EmailAttachments.Add(new EmailAttachmentEntity {
                    EmailId = emailEntity.Id,
                    AttachmentId = attachment.Id
                });
            }


            // Handle inline resources (similar to attachments)
            foreach (var inlineResource in email.InlineResources) {
       
                var lockObject = _inlineResourceLocks.GetOrAdd(inlineResource.Id, _ => {
                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}: inlineResource.Id = {inlineResource.Id} start lock");
                    return new object();
                });
                Console.WriteLine($"lockObject hashcode {lockObject.GetHashCode().ToString()}: inlineResource.Id = {inlineResource.Id} start lock");
                // Proper locking with object for each inline resource
                lock (lockObject) {
                    var existingInlineResource = dbContext.InlineResources
                        .FirstOrDefault(a => a.Id == inlineResource.Id);

                    if (existingInlineResource == null) {
                        var inlineResourceEntity = _mapper.Map<InlineResourceEntity>(inlineResource);
                        dbContext.InlineResources.Add(inlineResourceEntity);
                        dbContext.SaveChanges();
                    }

                    emailEntity.EmailInlineResources.Add(new EmailInlineResourceEntity {
                        EmailId = emailEntity.Id,
                        InlineResourceId = inlineResource.Id
                    });
                }
            }

            await dbContext.Emails.AddAsync(emailEntity);

            if (emailEntity.Status != null)
                dbContext.Entry(emailEntity.Status).State = EntityState.Unchanged;

            if (emailEntity.Requester != null)
                dbContext.Entry(emailEntity.Requester).State = EntityState.Unchanged;

            await dbContext.SaveChangesAsync();
            email.Id = emailEntity.Id;

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
