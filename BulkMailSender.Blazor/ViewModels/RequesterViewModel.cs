using BulkMailSender.Application.Dtos;

namespace BulkMailSender.Blazor.ViewModels {
    public class RequesterViewModel {
        public Guid Id { get; set; }
        public string LoginName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public Guid MailServerId { get; set; }
        public MailServerDto Server { get; set; } = new MailServerDto();                      
    }
}
