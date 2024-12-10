
public class FeedbackRecord : IVector
{
    [JsonPropertyName("Id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("PartnerShortName")]
    public string PartnerShortName { get; set; } = string.Empty;

    [JsonPropertyName("ServiceName")]
    public string ServiceName { get; set; } = string.Empty;

    [JsonPropertyName("Type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("Title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("Blocking")]
    public string Blocking { get; set; } = string.Empty;

    [JsonPropertyName("Description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("WorkaroundAvailable")]
    public string WorkaroundAvailable { get; set; } = string.Empty;

    [JsonPropertyName("Priority")]
    public string Priority { get; set; } = string.Empty;

    [JsonPropertyName("CustomerName")]
    public string CustomerName { get; set; } = string.Empty;

    [JsonPropertyName("CustomerTpid")]
    public string CustomerTpid { get; set; } = string.Empty;

    [JsonPropertyName("WorkaroundDescription")]
    public string WorkaroundDescription { get; set; } = string.Empty;

    [JsonPropertyName("UserStory")]
    public string UserStory { get; set; } = string.Empty;

    [JsonPropertyName("Embedding")]
    public float[] Embedding { get; set; } = [];// Embedding for the user story as a float array

    // Implement the GetVector method from IVector interface
    public float[] GetVector()
    {
        return Embedding ?? throw new InvalidOperationException("Embedding vector is not set.");
    }

    // A method to get a safe version of the FeedbackRecord (similar to GetSafeVersion in FunctionCodePair)
    public FeedbackRecord GetSafeVersion()
    {
        return new FeedbackRecord
        {
            Id = this.Id,
            PartnerShortName = this.PartnerShortName,
            ServiceName = this.ServiceName,
            Type = this.Type,
            Title = this.Title,
            Blocking = this.Blocking,
            Description = this.Description,
            WorkaroundAvailable = this.WorkaroundAvailable,
            Priority = this.Priority,
            CustomerName = this.CustomerName,
            CustomerTpid = this.CustomerTpid,
            WorkaroundDescription = this.WorkaroundDescription,
            UserStory = this.UserStory,
            Embedding = [] // We do not include the embedding in the safe version
        };
    }
}
