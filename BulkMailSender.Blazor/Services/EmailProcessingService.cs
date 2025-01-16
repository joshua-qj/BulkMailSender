using BulkMailSender.Blazor.ViewModels;
using System.Text.RegularExpressions;

namespace BulkMailSender.Blazor.Services {
    public class EmailProcessingService {
        public string ReplaceEmbeddedImagesWithCid(string emailBody, List<InlineResourceViewModel> embeddedImages) {
            foreach (var embeddedImage in embeddedImages) {
               // var base64String = $"data:image/png;base64,{Convert.ToBase64String(embeddedImage.Content)}";
                var base64String = $"data:{embeddedImage.MimeType};base64,{Convert.ToBase64String(embeddedImage.Content)}";
                var cidReference = $"cid:{embeddedImage.Id}";

  
                if (emailBody.Contains(base64String)) {   emailBody = emailBody.Replace(base64String, cidReference);
                } else {
                    Console.WriteLine($"Warning: Base64 image not found in email body for image ID: {embeddedImage.Id}");
                }
            }
            return emailBody;
        }

        public List<string> ExtractInlineResourceIds(string emailBody) {
            var inlineResourceIds = new List<string>();
            var matches = Regex.Matches(emailBody, @"cid:(?<id>[\w\-]+)");
            foreach (Match match in matches) {
                if (match.Groups["id"].Success) {
                    inlineResourceIds.Add(match.Groups["id"].Value);
                }
            }
            return inlineResourceIds;
        }
    }
}
