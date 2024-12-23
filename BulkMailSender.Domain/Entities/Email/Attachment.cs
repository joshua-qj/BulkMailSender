namespace BulkMailSender.Domain.Entities.Email
{
    public class Attachment
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public byte[] Content { get; set; }
        public override bool Equals(object? obj)
        {
            if (obj is not Attachment other) return false;

            // Customize comparison logic (e.g., compare by file name or content)
            return FileName == other.FileName && Content.SequenceEqual(other.Content);
        }

        public override int GetHashCode()
        {
            // Use a hash of the filename and content for uniqueness
            return HashCode.Combine(FileName, BitConverter.ToString(Content));
        }
    }
}
