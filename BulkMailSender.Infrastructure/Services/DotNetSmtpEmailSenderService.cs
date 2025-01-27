using BulkMailSender.Application.Interfaces.Email;
using BulkMailSender.Domain.Entities.Email;
using System.Net.Mail;
using System.Net.Mime;

namespace BulkMailSender.Infrastructure.Services {
    public class DotNetSmtpEmailSenderService : IEmailSenderService {
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
                    IsBodyHtml = email.IsBodyHtml
                };

                mailMessage.To.Add(email.EmailTo.Value);
                var htmlView = AlternateView.CreateAlternateViewFromString(mailMessage.Body, null, MediaTypeNames.Text.Html);

                //Add inline resources
                foreach (var resource in email.InlineResources) {
                    //switch (resource.MimeType) {
                    //    default:
                    //        break;
                    //}
                    //var linkedResource = new LinkedResource(new MemoryStream(resource.Content), MediaTypeNames.Image.Jpeg) {
                    //    ContentId = resource.Id.ToString(),
                    //    //ContentType = new ContentType("image/jpg"),
                    //    TransferEncoding = TransferEncoding.Base64
                    //};
                    string mimeType = resource.MimeType ?? "image/jpeg"; // Default to "image/jpeg" if MimeType is null

                    // Handle based on MIME type
                    switch (mimeType.ToLower()) {
                        case "image/jpeg":
                        case "image/png":
                        case "image/gif":
                        case "image/bmp":
                        case "image/svg+xml":
                        case "image/webp":
                            // If the resource is an image, use the respective MIME type
                            break;
                        case "image/x-icon":
                            // If it's an icon, use the icon MIME type
                            mimeType = "image/x-icon";
                            break;
                        case "application/pdf":
                            // If it's a PDF, use the PDF MIME type
                            mimeType = "application/pdf";
                            break;
                        // Add more cases for other types (e.g., video, audio, etc.)
                        default:
                            // Default case if the MIME type is not recognized, you can log or handle differently
                            mimeType = "application/octet-stream";
                            break;
                    }
                    //var mimeType = GetMimeTypeFromImageContent(resource.Content) ?? "image/jpeg";

                    var linkedResource = new LinkedResource(new MemoryStream(resource.Content), mimeType) {
                        ContentId = resource.Id.ToString(),
                        ContentLink = new Uri($"cid:{resource.Id}"),
                        TransferEncoding = TransferEncoding.Base64
                    };
                    linkedResource.ContentType.Name = resource.FileName ?? $"{resource.Id}.jpg";
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
               public static string GetMimeTypeFromImageContent(byte[] imageContent) {
            if (imageContent == null || imageContent.Length < 2)
                return null;

            // Checking for common image types based on their magic numbers (file signatures)
            var fileSignature = BitConverter.ToString(imageContent, 0, 4).Replace("-", "").ToLower();

            // JPEG (Start with FF D8 FF)
            if (fileSignature.StartsWith("ffd8")) {
                return "image/jpeg";
            }
            // PNG (Start with 89 50 4E 47)
            else if (fileSignature.StartsWith("89504e47")) {
                return "image/png";
            }
            // GIF (Start with 47 49 46 38)
            else if (fileSignature.StartsWith("47494638")) {
                return "image/gif";
            }
            // BMP (Start with 42 4D)
            else if (fileSignature.StartsWith("424d")) {
                return "image/bmp";
            }
            // WebP (Start with 52 49 46 46 00 00 00 57 45 42 50)
            else if (fileSignature.StartsWith("52494646") && imageContent.Length > 12 && BitConverter.ToString(imageContent, 8, 4).Replace("-", "").ToLower() == "57454250") {
                return "image/webp";
            }
            // TIFF (Start with 49 49 2A 00 or 4D 4D 00 2A)
            else if (fileSignature.StartsWith("49492a00") || fileSignature.StartsWith("4d4d002a")) {
                return "image/tiff";
            }
            // SVG (Start with <svg)
            else if (fileSignature.StartsWith("3c737667")) {
                return "image/svg+xml";
            }
            // ICO (Start with 00 00 01 00)
            else if (fileSignature.StartsWith("00000100")) {
                return "image/x-icon";
            }
            // Other types or unknown
            return null;
        }

        public Task<(bool IsSuccess, string ErrorMessage)> SendAsync(Email email, MailKit.Net.Smtp.SmtpClient? smtpClient = null) {
            throw new NotImplementedException();
        }
    }
}