using BulkMailSender.Application.Interfaces;
using BulkMailSender.Domain.Entities.Email;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

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
                var htmlView = AlternateView.CreateAlternateViewFromString(mailMessage.Body, null, MediaTypeNames.Text.Html);

                // Add inline resources
                foreach (var resource in email.InlineResources) {
                    var linkedResource = new LinkedResource(new MemoryStream(resource.Content),MediaTypeNames.Image.Jpeg) {
                        ContentId = resource.Id.ToString(),
                        //ContentType = new ContentType("image/jpg"),
                        TransferEncoding = TransferEncoding.Base64
                    };

                    htmlView.LinkedResources.Add(linkedResource);
                }

                mailMessage.AlternateViews.Add(htmlView);
                // Add attachments
                foreach (var attachment in email.Attachments) {
                    mailMessage.Attachments.Add(new System.Net.Mail.Attachment(new MemoryStream(attachment.Content), attachment.FileName));
                }

                await smtpClient.SendMailAsync(mailMessage);
                return (true, string.Empty);
            }
            catch (Exception ex) {
                return (false, $"Failed to send email: {ex.Message}");
            }
        }
    }
}