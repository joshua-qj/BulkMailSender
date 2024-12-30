using BulkMailSender.Application.Interfaces;
using BulkMailSender.Domain.Entities.Email;
using System.Net.Mail;

namespace BulkMailSender.Infrastructure.Services {
    public class SmtpEmailSenderService : IEmailSenderService {
        public async Task<(bool IsSuccess, string ErrorMessage)> SendAsync(Email email) {
            try {
                using var smtpClient = new SmtpClient(email.Requester.Server.ServerName) {
                    Port = email.Requester.Server.Port,
                    Credentials = new System.Net.NetworkCredential(email.Requester.LoginName, email.Requester.Password),
                    EnableSsl = email.Requester.Server.IsSecure
                };

                var mailMessage = new MailMessage {
                    From = new MailAddress(email.EmailFrom.Value, email.DisplayName),
                    Subject = email.Subject,
                    Body = email.Body,
                    IsBodyHtml = email.IsBodyHTML
                };

                mailMessage.To.Add(email.EmailTo.Value);

                // Add attachments
                //foreach (var attachment in email.Attachments) {
                //    mailMessage.Attachments.Add(new Attachment(new MemoryStream(attachment.AttachedContent), attachment.Name));
                //}

                // Add inline resources
                //foreach (var resource in email.InlineResources) {
                //    var linkedResource = new LinkedResource(new MemoryStream(resource.Content)) {
                //        ContentId = resource.Id.ToString()
                //    };
                //    var htmlView = AlternateView.CreateAlternateViewFromString(mailMessage.Body, null, MediaTypeNames.Text.Html);
                //    htmlView.LinkedResources.Add(linkedResource);
                //    mailMessage.AlternateViews.Add(htmlView);
                //}

                await smtpClient.SendMailAsync(mailMessage);
                return (true, string.Empty);
            }
            catch (Exception ex) {
                return (false, $"Failed to send email: {ex.Message}");
            }
        }
    }
}