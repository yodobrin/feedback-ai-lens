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
    // list of results
    /*
    * This method is used to find the best matches for a given query vector.
    * The strategy parameter is used to determine which comparison strategy to use.
    * The vectorSelector parameter is used to select the vector to compare against the query vector.
    * Results are sorted by similarity score and limited by maxResults and similarityThreshold.
    */
    private List<SearchResult> FindBestMatches(float[] query, Func<FeedbackRecord, float[]> vectorSelector, ComparisonStrategy strategy, int maxResults, float similarityThreshold)
    {
        long start = DateTime.Now.Ticks;

        // List to store the matching results
        List<(FeedbackRecord, float)> matches = new List<(FeedbackRecord, float)>();
Console.WriteLine($"Feedback records: {objects.Count}");
        // Evaluate similarity for each item in the collection
        for (int i = 0; i < objects.Count; i++)
        {
            float similarityScore = strategy(vectorSelector(objects[i]), query);

            // Only consider items that exceed the similarity threshold
            if (similarityScore >= similarityThreshold)
            {
                matches.Add((objects[i], similarityScore));
            }
        }

        // Sort by similarity score in descending order
        matches.Sort((a, b) => b.Item2.CompareTo(a.Item2));

        // Limit the number of results by maxResults
        List<SearchResult> topResults = new List<SearchResult>();
        for (int i = 0; i < Math.Min(matches.Count, maxResults); i++)
        {
            topResults.Add(new SearchResult(matches[i].Item1.GetSafeVersion(), matches[i].Item2, (float)(DateTime.Now.Ticks - start) / TimeSpan.TicksPerMillisecond));
        }

        return topResults;
    }

    public List<SearchResult> FindByDotProduct(float[] query, Func<FeedbackRecord, float[]> vectorSelector, int maxResults, float similarityThreshold)
    {
        return FindBestMatches(query, vectorSelector, VectorMath.DotProduct, maxResults, similarityThreshold);
    }

    public List<SearchResult> FindByCosineSimilarity(float[] query, Func<FeedbackRecord, float[]> vectorSelector, int maxResults, float similarityThreshold)
    {
        return FindBestMatches(query, vectorSelector, VectorMath.CosineSimilarity, maxResults, similarityThreshold);
    }

    public List<SearchResult> FindByEuclideanDistance(float[] query, Func<FeedbackRecord, float[]> vectorSelector, int maxResults, float similarityThreshold)
    {
        return FindBestMatches(query, vectorSelector, (a, b) => -VectorMath.EuclideanDistance(a, b), maxResults, similarityThreshold);
    }    
}