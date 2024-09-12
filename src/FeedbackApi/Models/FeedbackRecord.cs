
public class FeedbackRecord : IVector
{
    [JsonPropertyName("Id")]
    public string Id { get; set; }

    [JsonPropertyName("PartnerShortName")]
    public string PartnerShortName { get; set; }

    [JsonPropertyName("ServiceName")]
    public string ServiceName { get; set; }

    [JsonPropertyName("Type")]
    public string Type { get; set; }

    [JsonPropertyName("Title")]
    public string Title { get; set; }

    [JsonPropertyName("Blocking")]
    public string Blocking { get; set; }

    [JsonPropertyName("Description")]
    public string Description { get; set; }

    [JsonPropertyName("WorkaroundAvailable")]
    public string WorkaroundAvailable { get; set; }

    [JsonPropertyName("Priority")]
    public string Priority { get; set; }

    [JsonPropertyName("CustomerName")]
    public string CustomerName { get; set; }

    [JsonPropertyName("CustomerTpid")]
    public string CustomerTpid { get; set; }

    [JsonPropertyName("WorkaroundDescription")]
    public string WorkaroundDescription { get; set; }

    [JsonPropertyName("UserStory")]
    public string UserStory { get; set; }

    [JsonPropertyName("Embedding")]
    public float[] Embedding { get; set; } // Embedding for the user story as a float array

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
            Embedding = null // We do not include the embedding in the safe version
        };
    }
}