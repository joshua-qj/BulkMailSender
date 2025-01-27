using BulkMailSender.Application.Dtos;
using System.Collections.Concurrent;

namespace BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces {
    public interface ISendEmailsUseCase {
        Task ExecuteAsync(IEnumerable<EmailDto> emails , DateTime? startTime, int? emailCount,RequesterDto? requesterDto, ConcurrentDictionary<Guid, EmailFailureRecord>? failedEmails = null) ;
    }
}