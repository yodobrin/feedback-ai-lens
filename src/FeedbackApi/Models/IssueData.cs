public class IssueData
{
    [JsonPropertyName("original_user_prompt")]
    public string OriginalUserPrompt { get; set; }  // Optional field to store the original user query

    [JsonPropertyName("user_story")]
    public string UserStory { get; set; }

    [JsonPropertyName("customers")]
    public List<Customer> Customers { get; set; }
}