using BulkMailSender.Application.Dtos;
using BulkMailSender.Application.Interfaces.Email;
using BulkMailSender.Application.UseCases.Email.EmailJobSummaryScreen.interfaces;

namespace BulkMailSender.Application.UseCases.Email.EmailJobSummaryScreen {
    public class GetJobSummariesUseCase : IGetJobSummariesUseCase {
        private readonly IEmailRepository _emailRepository;

        public GetJobSummariesUseCase(IEmailRepository emailRepository) {
            _emailRepository = emailRepository;
        }

        public IQueryable<JobSummaryDto> Execute(Guid userId) {
            return _emailRepository.GetJobSummaries(userId);
        }
    }
}
