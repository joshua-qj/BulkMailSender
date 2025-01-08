namespace BulkMailSender.Blazor.Services {
    public class FileHelperService {
        public string? GetMimeTypeFromImageContent(byte[] imageContent) {
            if (imageContent == null || imageContent.Length < 4)
                return null;

            if (imageContent[0] == 0xFF && imageContent[1] == 0xD8)
                return "image/jpeg";

            if (imageContent[0] == 0x89 && imageContent[1] == 0x50 &&
                imageContent[2] == 0x4E && imageContent[3] == 0x47)
                return "image/png";

            if (imageContent[0] == 0x47 && imageContent[1] == 0x49 &&
                imageContent[2] == 0x46 && (imageContent[3] == 0x38))
                return "image/gif";

            if (imageContent[0] == 0x42 && imageContent[1] == 0x4D)
                return "image/bmp";

            if (imageContent[0] == 0x00 && imageContent[1] == 0x00 &&
                imageContent[2] == 0x01 && imageContent[3] == 0x00)
                return "image/x-icon";

            if (imageContent.Length >= 5 &&
                ((imageContent[0] == 0x3C && imageContent[1] == 0x3F && imageContent[2] == 0x78 &&
                  imageContent[3] == 0x6D && imageContent[4] == 0x6C) ||
                 (imageContent[0] == 0x3C && imageContent[1] == 0x73 && imageContent[2] == 0x76 &&
                  imageContent[3] == 0x67)))
                return "image/svg+xml";

            return null;
        }
    }
}
