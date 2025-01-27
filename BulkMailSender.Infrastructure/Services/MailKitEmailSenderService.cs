using BulkMailSender.Application.Interfaces.Email;
using BulkMailSender.Domain.Entities.Email;
using MailKit.Security;
using MimeKit;
using System.Net.Mail;

namespace BulkMailSender.Infrastructure.Services {
    public class MailKitEmailSenderService : IEmailSenderService {
        public async Task<(bool IsSuccess, string ErrorMessage)> SendAsync(Email email, MailKit.Net.Smtp.SmtpClient? smtpClient = null) {
            bool isSmtpClientCreatedLocally =  smtpClient == null;
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

                // Send the email using MailKit's SmtpClient

                //if (smtpClient == null) {
                //    smtpClient = new MailKit.Net.Smtp.SmtpClient();
                //    isSmtpClientCreatedLocally = true;

                    // Connect to the SMTP server
                //    await smtpClient.ConnectAsync(email.Requester.Server.ServerName, email.Requester.Server.Port, SecureSocketOptions.StartTls);
                //    await smtpClient.AuthenticateAsync(email.Requester.LoginName, email.Requester.Password);
                //}
                //using var smtpClient = new MailKit.Net.Smtp.SmtpClient();
                //await smtpClient.ConnectAsync(email.Requester.Server.ServerName, email.Requester.Server.Port, SecureSocketOptions.StartTls);
                //await smtpClient.AuthenticateAsync(email.Requester.LoginName, email.Requester.Password);
                await smtpClient.SendAsync(message);
                //await smtpClient.DisconnectAsync(true);

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

        public static string GetMimeTypeFromImageContent(byte[] imageContent) {
            if (imageContent == null || imageContent.Length < 4)
                return null;

            // Checking file signatures for common image formats
            var fileSignature = BitConverter.ToString(imageContent, 0, 4).Replace("-", "").ToLower();

            return fileSignature switch {
                "ffd8" => "image/jpeg", // JPEG
                "89504e47" => "image/png", // PNG
                "47494638" => "image/gif", // GIF
                "424d" => "image/bmp", // BMP
                "52494646" when imageContent.Length > 12 && BitConverter.ToString(imageContent, 8, 4).Replace("-", "").ToLower() == "57454250" => "image/webp", // WebP
                "49492a00" or "4d4d002a" => "image/tiff", // TIFF
                "00000100" => "image/x-icon", // ICO
                _ => null // Unknown type
            };
        }
    }
}