namespace BulkMailSender.Domain.Entities.Email
{
    public class InlineResource
    {
        public Guid Id { get; set; }
        public byte[] Content { get; set; }
        public string? FileName { get; set; } // Optional: The file name of the resource
        public string? MimeType { get; set; } // The MIME type (e.g., image/png, application/pdf, etc.)     

        public override bool Equals(object? obj)
        {
            if (obj is not InlineResource other) return false;

            // Customize comparison logic for uniqueness
            return Id == other.Id &&
                   Content.SequenceEqual(other.Content) &&
                   FileName == other.FileName &&
                   MimeType == other.MimeType;
        }

        public override int GetHashCode()
        {
            // Combine relevant fields for uniqueness
            return HashCode.Combine(Id, FileName, MimeType, BitConverter.ToString(Content));
        }
    }
}
