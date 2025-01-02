namespace BulkMailSender.Blazor.Services {
    //public static class ChunkService {
    //    public static List<List<T>> ChunkList<T>(List<T> source, int chunkSize) {
    //        var chunks = new List<List<T>>();
    //        for (int i = 0; i < source.Count; i += chunkSize) {
    //            chunks.Add(source.GetRange(i, Math.Min(chunkSize, source.Count - i)));
    //        }
    //        return chunks;
    //    }
    //}
    public static class ChunkService {
        public static IEnumerable<List<T>> ChunkList<T>(List<T> source, int chunkSize) {
            for (int i = 0; i < source.Count; i += chunkSize) {
                yield return source.GetRange(i, Math.Min(chunkSize, source.Count - i));
            }
        }
    }
}