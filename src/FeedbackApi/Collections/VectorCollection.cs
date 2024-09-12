// using System.Text.Json;
// using System.IO;
// using System.Collections.Generic;
// using System.Threading.Tasks;

public class VectorCollection
{
    private readonly int dimensions;

    private List<FeedbackRecord> objects = new List<FeedbackRecord>();

    public async Task SaveToDiskAsync(string path)
    {
        string json = JsonSerializer.Serialize(objects, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(path, json);
    }

    // Return the objects as a list
    public List<FeedbackRecord> GetFeedbackRecords()
    {
        return objects;
    }

    public static async Task<VectorCollection> CreateFromMemoryAsync(Stream dataStream)
    {
        long start = DateTime.Now.Ticks;
        using var reader = new StreamReader(dataStream);
        string jsonFromStream = await reader.ReadToEndAsync();
        List<FeedbackRecord> loadedObjects = JsonSerializer.Deserialize<List<FeedbackRecord>>(jsonFromStream) ?? new List<FeedbackRecord>();

        var collection = new VectorCollection(1536);
        collection.AddRange(loadedObjects);
        long endtime = DateTime.Now.Ticks;
        Console.WriteLine($"Time to load data from memory: {(float)(endtime - start) / TimeSpan.TicksPerMillisecond} ms");
        return collection;
    }

    public static async Task<VectorCollection> CreateFromDiskAsync(string path)
    {
        string jsonFromFile = await File.ReadAllTextAsync(path);
        List<FeedbackRecord> loadedObjects = JsonSerializer.Deserialize<List<FeedbackRecord>>(jsonFromFile) ?? new List<FeedbackRecord>();
        var collection = new VectorCollection(1536);
        collection.AddRange(loadedObjects);
        return collection;
    }

    public VectorCollection(int dimensions)
    {
        this.dimensions = dimensions;
    }

    public int Dimensions => dimensions;

    public void Add(FeedbackRecord obj)
    {
        objects.Add(obj);
    }

    public void AddRange(IEnumerable<FeedbackRecord> _objects)
    {
        objects.AddRange(_objects);
    }

    public IVector GetItem(int index)
    {
        return objects[index];
    }

    private delegate float ComparisonStrategy(float[] vectorA, float[] vectorB);

    /*
    * This method is used to find the best match for a given query vector.
    * The strategy parameter is used to determine which comparison strategy to use.
    * The vectorSelector parameter is used to select the vector to compare against the query vector.
    */
    private SearchResult FindBestMatch(float[] query, Func<FeedbackRecord, float[]> vectorSelector, ComparisonStrategy strategy)
    {
        long start = DateTime.Now.Ticks;
        float bestValue = float.MinValue;
        int bestIndex = 0;

        for (int i = 0; i < objects.Count; i++)
        {
            float currentValue = strategy(vectorSelector(objects[i]), query);
            if (currentValue > bestValue)
            {
                bestValue = currentValue;
                bestIndex = i;
            }
        }
        long endtime = DateTime.Now.Ticks;

        return new SearchResult(objects[bestIndex].GetSafeVersion(), bestValue, (float)(endtime - start) / TimeSpan.TicksPerMillisecond);
    }

    public SearchResult FindByDotProduct(float[] query, Func<FeedbackRecord, float[]> vectorSelector)
    {
        return FindBestMatch(query, vectorSelector, VectorMath.DotProduct);
    }

    public SearchResult FindByCosineSimilarity(float[] query, Func<FeedbackRecord, float[]> vectorSelector)
    {
        return FindBestMatch(query, vectorSelector, VectorMath.CosineSimilarity);
    }

    public SearchResult FindByEuclideanDistance(float[] query, Func<FeedbackRecord, float[]> vectorSelector)
    {
        return FindBestMatch(query, vectorSelector, (a, b) => -VectorMath.EuclideanDistance(a, b));
    }
}