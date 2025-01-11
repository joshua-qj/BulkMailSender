using BulkMailSender.Application.Dtos;

namespace BulkMailSender.Application.UseCases.Email.EmailJobSummaryScreen.interfaces {
    public interface IGetJobSummariesUseCase {
        IQueryable<JobSummaryDto> Execute(Guid userId);
    }
}