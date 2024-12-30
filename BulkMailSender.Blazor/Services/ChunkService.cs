namespace BulkMailSender.Blazor.Services {
    public static class ChunkService {
        public static IEnumerable<List<T>> ChunkList<T>(List<T> source, int chunkSize) {
            for (int i = 0; i < source.Count; i += chunkSize) {
                yield return source.GetRange(i, Math.Min(chunkSize, source.Count - i));
            }
        }
    }
}