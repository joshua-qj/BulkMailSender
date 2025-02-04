using BulkMailSender.Application.Interfaces.Email;
using BulkMailSender.Domain.Entities.Email;
using MailKit.Security;
using MimeKit;

namespace BulkMailSender.Infrastructure.Services {
    public class MailKitEmailSenderService : IEmailSenderService {
        public async Task<(bool IsSuccess, string ErrorMessage)> SendAsync(Email email, MailKit.Net.Smtp.SmtpClient? smtpClient = null) {
            bool isSmtpClientCreatedLocally = smtpClient == null;

            try {
                if (isSmtpClientCreatedLocally) {
                    smtpClient = new MailKit.Net.Smtp.SmtpClient();
                    await smtpClient.ConnectAsync(email.Requester.Server.ServerName, email.Requester.Server.Port, SecureSocketOptions.StartTls);
                    await smtpClient.AuthenticateAsync(email.Requester.LoginName, email.Requester.Password);
                }
                var message = new MimeMessage();

                // Set the sender and recipient
                message.From.Add(new MailboxAddress(email.DisplayName, email.EmailFrom.Value));
                message.To.Add(new MailboxAddress(email.EmailTo.Value, email.EmailTo.Value));
                message.Subject = email.Subject;

                // Create a body builder for the email content
                var bodyBuilder = new BodyBuilder {
                    HtmlBody = email.Body
                };

                // Add inline resources
                foreach (var resource in email.InlineResources) {
                    string mimeType = resource.MimeType ?? "image/jpeg";

                    // Exclude unsupported types if needed
                    if (!IsSupportedInlineMimeType(mimeType))
                        continue;

                    // Create a MimePart for the inline resource
                    var inlinePart = new MimePart(mimeType) {
                        Content = new MimeContent(new MemoryStream(resource.Content)),
                        ContentId = resource.Id.ToString(),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        IsAttachment = false,
                        ContentBase = new Uri($"cid:{resource.Id}"),
                        ContentLocation = new Uri($"cid:{resource.Id}"),
                        ContentDisposition = new MimeKit.ContentDisposition(MimeKit.ContentDisposition.Inline),
                        FileName = resource.FileName ?? $"{resource.Id}.jpg"
                    };

                    bodyBuilder.LinkedResources.Add(inlinePart);
                }

                // Add attachments
                foreach (var attachment in email.Attachments) {
                    var attachmentPart = new MimePart {
                        IsAttachment = true,
                        Content = new MimeContent(new MemoryStream(attachment.Content)),
                        ContentDisposition = new MimeKit.ContentDisposition(MimeKit.ContentDisposition.Attachment),
                        FileName = attachment.FileName,
                        ContentTransferEncoding = MimeKit.ContentEncoding.Base64
                    };

                    bodyBuilder.Attachments.Add(attachmentPart);
                }

                // Set the final email body
                message.Body = bodyBuilder.ToMessageBody();
                //await _sendSemaphore.WaitAsync();
                try {
                    //   Console.WriteLine($"Thread {Task.CurrentId} enters inside smtpClient section at {DateTime.Now:HH:mm:ss.fff}");
                    Console.WriteLine($"using smtpClient HashCode {smtpClient?.GetHashCode().ToString()}  at {DateTime.Now:HH:mm:ss.fff}");
                    if (smtpClient.IsConnected) {

                        await smtpClient.SendAsync(message);
                    } else {
                        Console.WriteLine($"using smtpClient HashCode {smtpClient?.GetHashCode().ToString()} failed at {DateTime.Now:HH:mm:ss.fff}");

                    }
                    // Console.WriteLine($"Thread {Task.CurrentId} exits smtpClient section at {DateTime.Now:HH:mm:ss.fff}");
                }
                catch (Exception ex) {
                    return (false, $"Failed to send email: {ex.Message}");
                }
                finally {
                    //_sendSemaphore.Release();
                }

                return (true, string.Empty);

            }
            catch (Exception ex) {
                return (false, $"Failed to send email: {ex.Message}");
            }
            finally {
                // If the smtpClient was created locally, disconnect and dispose it
                if (isSmtpClientCreatedLocally && smtpClient != null) {
                    await smtpClient.DisconnectAsync(true);
                    smtpClient.Dispose();
                }
            }
        }

        private bool IsSupportedInlineMimeType(string mimeType) {
            // List of supported MIME types for inline resources
            var supportedInlineMimeTypes = new[]
            {
            "image/jpeg", "image/png", "image/gif", "image/x-icon","image/bmp", "image/svg+xml", "image/webp"
        };
            return supportedInlineMimeTypes.Contains(mimeType.ToLower());
        }

    }
}
