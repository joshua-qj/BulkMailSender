using AutoMapper;
using BulkMailSender.Application.Dtos;
using BulkMailSender.Application.Interfaces.Email;
using BulkMailSender.Domain.Entities.Email;
using BulkMailSender.Infrastructure.Common.Entities.Email;
using BulkMailSender.Infrastructure.InMemoryPersistence.Contexts;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;

namespace BulkMailSender.Infrastructure.InMemoryPersistence.Repositories {
    public class InMemoryEmailRepository : IEmailRepository {
        private readonly IDbContextFactory<InMemoryDbContext> _contextFactory;
        private readonly InMemoryDbContext _dbContext;
        private readonly IMapper _mapper;

        public InMemoryEmailRepository(InMemoryDbContext dbContext, 
            IMapper mapper, 
            IDbContextFactory<InMemoryDbContext> contextFactory) {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _contextFactory = contextFactory;
        }

        public async Task<Attachment> FindOrAddAttachmentAsync(Attachment attachment) {
            if (attachment == null)
                throw new ArgumentNullException(nameof(attachment));

            var existingAttachment = await _dbContext.Attachments
                .FirstOrDefaultAsync(a => a.FileName == attachment.FileName && a.Content == attachment.Content);
            
            if (existingAttachment == null) {
                var attachmentEntity = _mapper.Map<AttachmentEntity>(attachment);
                await _dbContext.Attachments.AddAsync(attachmentEntity);
                await _dbContext.SaveChangesAsync();
                return _mapper.Map<Attachment>(attachmentEntity);
            }

            return _mapper.Map<Attachment>(existingAttachment);
        }

        public async Task<InlineResource> FindOrAddInlineResourceAsync(InlineResource inlineResource) {
            if (inlineResource == null)
                throw new ArgumentNullException(nameof(inlineResource));

            var existingInlineResource = await _dbContext.InlineResources
                .FirstOrDefaultAsync(ir => ir.FileName == inlineResource.FileName && ir.Content == inlineResource.Content);

            if (existingInlineResource == null) {
                var inlineResourceEntity = _mapper.Map<InlineResourceEntity>(inlineResource);
                await _dbContext.InlineResources.AddAsync(inlineResourceEntity);
                await _dbContext.SaveChangesAsync();
                return _mapper.Map<InlineResource>(inlineResourceEntity);
            }

            return _mapper.Map<InlineResource>(existingInlineResource);
        }

        public async Task<Email?> GetEmailByIdAsync(Guid emailId) {
            if (emailId == Guid.Empty)
                return null;

            var emailEntity = await _dbContext.Emails
                .Include(email => email.EmailAttachments)
                    .ThenInclude(emailAttachment => emailAttachment.Attachment)
                .Include(email => email.EmailInlineResources)
                    .ThenInclude(emailInlineResource => emailInlineResource.InlineResource)
                .AsNoTracking()
                .FirstOrDefaultAsync(email => email.Id == emailId);

            return _mapper.Map<Email?>(emailEntity);
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
            var requesterEntity = await _dbContext.Requesters
                .Include(r => r.MailServer)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == requesterId);

            return _mapper.Map<Requester>(requesterEntity);
        }

        public async Task<Requester> GetRequesterByNameAsync(string hostName) {
            var requesterEntity = await _dbContext.Requesters
                .Include(r => r.MailServer)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.LoginName == hostName);

            return _mapper.Map<Requester>(requesterEntity);
        }

        public async Task LoadRequesterConfigurationsAsync() {
            var requesterEntities = await _dbContext.Requesters
                .Include(r => r.MailServer)
                .AsNoTracking()
                .ToListAsync();

            _ = requesterEntities.Select(re => _mapper.Map<Requester>(re)).ToList();
        }

        public async Task<byte[]> ReadFileAsBytesUseCaseAsync(Stream fileStream) {
            if (fileStream == null)
                throw new ArgumentNullException(nameof(fileStream));

            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        public async Task<Email> SaveEmailAsync(Email email) {
            using var dbContext = _contextFactory.CreateDbContext();
            if (email == null)
                throw new ArgumentNullException(nameof(email));

            var emailEntity = _mapper.Map<EmailEntity>(email);

            foreach (var attachment in email.Attachments) {
                var attachmentEntity = _mapper.Map<AttachmentEntity>(attachment);
                await FindOrAddAttachmentAsync(attachment);
                emailEntity.EmailAttachments.Add(new EmailAttachmentEntity {
                    EmailId = emailEntity.Id,
                    AttachmentId = attachment.Id
                });
            }

            foreach (var inlineResource in email.InlineResources) {
                var inlineResourceEntity = _mapper.Map<InlineResourceEntity>(inlineResource);
                await FindOrAddInlineResourceAsync(inlineResource);
                emailEntity.EmailInlineResources.Add(new EmailInlineResourceEntity {
                    EmailId = emailEntity.Id,
                    InlineResourceId = inlineResource.Id
                });
            }

            dbContext.Emails.Add(emailEntity);
            if (emailEntity.Status != null) {
                dbContext.Entry(emailEntity.Status).State = EntityState.Unchanged;
            }
            if (emailEntity.Requester != null) {
                dbContext.Entry(emailEntity.Requester).State = EntityState.Unchanged;
            }
            await dbContext.SaveChangesAsync();
            email.Id = emailEntity.Id;
            return email;
        }

        public async Task UpdateEmailStatusAsync(Email email, string? errorMessage) {
            using var dbContext = _contextFactory.CreateDbContext();
            var emailEntity = await _dbContext.Emails.FindAsync(email.Id);

            if (emailEntity == null)
                throw new KeyNotFoundException("Email not found.");

            emailEntity.StatusId = email.StatusId;
            emailEntity.ErrorMessage = errorMessage;
            dbContext.Emails.Update(emailEntity);
            await dbContext.SaveChangesAsync();
        }
    }
}
