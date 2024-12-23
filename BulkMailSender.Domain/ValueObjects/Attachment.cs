namespace BulkMailSender.Domain.ValueObjects
{
    public class Attachment
    {
        private const int MaxSizeInBytes = 5 * 1024 * 1024; // 5 MB
        public string FileName { get; }
        public byte[] Content { get; }

        public Attachment(string fileName, byte[] content)
        {
            if (content.Length > MaxSizeInBytes)
                throw new ArgumentException($"Attachment size cannot exceed {MaxSizeInBytes / (1024 * 1024)} MB.", nameof(content));

            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public override bool Equals(object? obj) =>
            obj is Attachment other && FileName == other.FileName && Content.SequenceEqual(other.Content);

        public override int GetHashCode() => HashCode.Combine(FileName, Content);
    }
}
