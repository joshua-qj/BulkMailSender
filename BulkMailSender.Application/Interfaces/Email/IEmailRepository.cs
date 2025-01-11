using BulkMailSender.Application.Dtos;
using BulkMailSender.Domain.Entities.Email;
namespace BulkMailSender.Application.Interfaces.Email {
    public interface IEmailRepository {
        Task<Domain.Entities.Email.Email> SaveEmailAsync(Domain.Entities.Email.Email email);
        Task<Requester> GetRequesterByIdAsync(Guid requesterId);
        Task LoadRequesterConfigurationsAsync();
        Task<Requester> GetRequesterByNameAsync(string hostName);
 
        Task UpdateEmailStatusAsync(Domain.Entities.Email.Email email, string? errorMessage);
        IQueryable<JobSummaryDto> GetJobSummaries(Guid userId);
    }
}
/*Repositories are focused on persisting and retrieving domain models.
The mapping to DTOs is a responsibility of the application layer.*/