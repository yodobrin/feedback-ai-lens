public class IssueData
{
    [JsonPropertyName("original_user_prompt")]
    public string OriginalUserPrompt { get; set; } = string.Empty; // Optional field to store the original user query

    [JsonPropertyName("summary_detail")]
    public SummaryDetail UserStory { get; set; } = new SummaryDetail();

    [JsonPropertyName("customers")]
    public List<Customer> Customers { get; set; } = new List<Customer>();
}
