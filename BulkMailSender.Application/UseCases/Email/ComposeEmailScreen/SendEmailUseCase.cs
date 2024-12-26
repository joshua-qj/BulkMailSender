using BulkMailSender.Application.Dtos;
using BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace EmailSender.UseCases.EmailCompaigns.ComposeEmailScreen {
    public class SendEmailUseCase : ISendEmailUseCase
    {
        private readonly IGetRequesterByIdUseCase _getRequesterByIdUseCase;


        public SendEmailUseCase(IGetRequesterByIdUseCase getRequesterByIdUseCase)

        {
            _getRequesterByIdUseCase = getRequesterByIdUseCase;

        }

        public async Task<(bool IsSuccess, string ErrorMessage)> ExecuteAsync(EmailDto email)
        {
            try
            {
                // Fetch requester SMTP configuration
                var requester = await _getRequesterByIdUseCase.GetRequesterByIdAsync(email.RequesterID);
                if (requester == null)
                {
                    const string errorMessage = "Requester configuration not found.";
                    return (false, errorMessage);
                }

                // Define SMTP settings based on the requester configuration
                using var smtpClient = new SmtpClient(requester.Server.ServerName)
                {
                    Port = requester.Server.Port,
                    Credentials = new NetworkCredential(requester.LoginName, requester.Password),
                    EnableSsl = requester.Server.EnableSsl
                };

                // Set up the email message
                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(email.EmailFrom, email.DisplayName),
                    Subject = email.Subject,
                    Body = email.Body,
                    IsBodyHtml = email.IsBodyHtml ?? false, // Default to false if null
                };

                //        if EmailTo is a list, Add recipients
                //foreach (var recipient in email.EmailTo.Split(';', StringSplitOptions.RemoveEmptyEntries))
                //{
                //    mailMessage.To.Add(recipient.Trim());
                //}

                // Prepare the AlternateView for inline images
                var htmlView = AlternateView.CreateAlternateViewFromString(mailMessage.Body, null, MediaTypeNames.Text.Html);

                // Add inline images from EmbededImages
                foreach (var embeddedImage in email.InlineResources)
                {
                    var linkedResource = new LinkedResource(new MemoryStream(embeddedImage.Content), MediaTypeNames.Image.Jpeg)
                    {
                        //ContentId = embeddedImage.Id.ToString(), // Ensure this matches the cid in your HTML
                        TransferEncoding = TransferEncoding.Base64 // Encode inline image
                    };

                    htmlView.LinkedResources.Add(linkedResource);
                }

                // Attach AlternateView to MailMessage
                mailMessage.AlternateViews.Add(htmlView);

                // Add any attachments
                foreach (var attachment in email.Attachments)
                {
                    mailMessage.Attachments.Add(new System.Net.Mail.Attachment(new MemoryStream(attachment.Content), attachment.FileName));
                }

                // Send the email
                await smtpClient.SendMailAsync(mailMessage);

                // Email sent successfully
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                // Log the error
                var errorMessage = $"Error sending email: {ex.Message}";

                return (false, errorMessage);
            }
        }
    }
}
