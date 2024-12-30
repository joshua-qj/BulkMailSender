using BulkMailSender.Application.Dtos;
using BulkMailSender.Domain.Entities.Email;

namespace BulkMailSender.Application.Interfaces {
    public interface IEmailRepository {
        Task<Email> SaveEmailAsync(Email email);
        Task<Requester> GetRequesterByIdAsync(Guid requesterId);
        Task LoadRequesterConfigurationsAsync();
        Task<Requester> GetRequesterByNameAsync(string hostName);
        Task<Attachment> FindOrAddAttachmentAsync(Attachment attachment);

        Task<InlineResource> FindOrAddInlineResourceAsync(InlineResource inlineResource);
        IQueryable<JobSummary> GetGroupedEmails(Guid userId);
        Task<byte[]> ReadFileAsBytesUseCaseAsync(Stream fileStream);
        Task UpdateEmailStatusAsync(EmailDto emailDto, string? errorMessage);
    }
}
/*Repositories are focused on persisting and retrieving domain models.
The mapping to DTOs is a responsibility of the application layer.*/