using CsvHelper.Configuration.Attributes;

public class CSVFeedbackRecord
{
    public string Id { get; set; }

    public string PartnerShortName { get; set; }

    [Name("ServiceTree_Name")]
    public string ServiceName { get; set; }

    public string Type { get; set; }

    public string Title { get; set; }

    public string Blocking { get; set; }

    [Name("CleanDescription")]
    public string Description { get; set; }

    public string WorkaroundAvailable { get; set; }

    public string Priority { get; set; }

    [Name("Customer_Name")]
    public string CustomerName { get; set; }

    [Name("Customer_Tpid")]
    public string CustomerTpid { get; set; }

    [Name("CleanWorkaroundDescription")]
    public string WorkaroundDescription { get; set; }

    // Field to store the generated user story
    [Optional]
    public string UserStory { get; set; }
    
    [Optional]
    public float[] Embedding { get; set; } // Embedding for the user story as a float array

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