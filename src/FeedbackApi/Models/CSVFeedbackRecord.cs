using CsvHelper.Configuration.Attributes;

public class CSVFeedbackRecord
{
    public string Id { get; set; } = string.Empty;

    public string PartnerShortName { get; set; } = string.Empty;

    [Name("ServiceTree_Name")]
    public string ServiceName { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Blocking { get; set; } = string.Empty;

    [Name("CleanDescription")]
    public string Description { get; set; } = string.Empty;

    public string WorkaroundAvailable { get; set; } = string.Empty;

    public string Priority { get; set; } = string.Empty;

    [Name("Customer_Name")]
    public string CustomerName { get; set; } = string.Empty;

    [Name("Customer_Tpid")]
    public string CustomerTpid { get; set; } = string.Empty;

    [Name("CleanWorkaroundDescription")]
    public string WorkaroundDescription { get; set; } = string.Empty;

    // Field to store the generated user story
    [Optional]
    public string UserStory { get; set; } = string.Empty;

    [Optional]
    public float[] Embedding { get; set; } = Array.Empty<float>();// Embedding for the user story as a float array

    public string ToPrompt()
    {
        return $"Service: {ServiceName}\n" +
               $"Title: {Title}\n" +
               $"Description: {Description}\n" +
               $"Blocking: {Blocking}\n" +
               $"Workaround Available: {WorkaroundAvailable}\n" +
               $"Priority: {Priority}\n" +
               $"Customer: {CustomerName}\n" +
               $"UserStory: \n\n" ;
    }
}
