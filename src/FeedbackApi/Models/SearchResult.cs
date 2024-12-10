public class SearchResult
{
    public FeedbackRecord Item { get; set; }
    public float Value { get; set; }
    public float Ms { get; set; }

    public SearchResult(FeedbackRecord item, float value, float ms)
    {
        Item = item;
        Value = value;
        Ms = ms;
    }
}
