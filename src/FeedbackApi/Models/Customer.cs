public class Customer
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("industry")]
    public string Industry { get; set; } = string.Empty;

    [JsonPropertyName("tpid")]
    public string Tpid { get; set; } = string.Empty;

    [JsonPropertyName("feedback_title")]
    public string FeedbackTitle { get; set; } = string.Empty;

    [JsonPropertyName("feedback_records")]
    public List<FeedbackRecord> FeedbackRecords { get; set; } = new List<FeedbackRecord>();

    [JsonPropertyName("summary_detail")]
    public SummaryDetail SummaryDetail { get; set; } = new SummaryDetail();
}
