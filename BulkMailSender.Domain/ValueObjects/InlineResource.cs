namespace BulkMailSender.Domain.ValueObjects
{
    public class InlineResource
    {
        private const int MaxSizeInBytes = 2 * 1024 * 1024; // 2 MB
        public string ResourceName { get; }
        public byte[] Content { get; }

        public InlineResource(string resourceName, byte[] content)
        {
            if (content.Length > MaxSizeInBytes)
                throw new ArgumentException($"Inline resource size cannot exceed {MaxSizeInBytes / (1024 * 1024)} MB.", nameof(content));

            ResourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public override bool Equals(object? obj) =>
            obj is InlineResource other && ResourceName == other.ResourceName && Content.SequenceEqual(other.Content);

        public override int GetHashCode() => HashCode.Combine(ResourceName, Content);
    }

}
