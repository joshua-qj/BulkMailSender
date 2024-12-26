using BulkMailSender.Application.Interfaces;
using BulkMailSender.Domain.Entities.Email;

namespace BulkMailSender.Infrastructure.InMemoryPersistence.Repositories {
    public class InMemoryEmailRepository : IEmailRepository {
        public Task<Attachment> FindOrAddAttachmentAsync(Attachment attachment) {
            throw new NotImplementedException();
        }

        public Task<InlineResource> FindOrAddInlineResourceAsync(InlineResource inlineResource) {
            throw new NotImplementedException();
        }

        public IQueryable<JobSummary> GetGroupedEmails(Guid userId) {
            throw new NotImplementedException();
        }

        public Task<Requester> GetRequesterByIdAsync(Guid requesterId) {
            throw new NotImplementedException();
        }

        public Task<Requester> GetRequesterByNameAsync(string hostName) {
            throw new NotImplementedException();
        }

        public Task LoadRequesterConfigurationsAsync() {
            throw new NotImplementedException();
        }

        public Task<byte[]> ReadFileAsBytesUseCaseAsync(Stream fileStream) {
            throw new NotImplementedException();
        }

        public Task<Email> SaveEmailAsync(Email email) {
            throw new NotImplementedException();
        }

        public Task UpdateEmailStatusAsync(Guid emailId, string? errorMessage) {
            throw new NotImplementedException();
        }
    }
}
