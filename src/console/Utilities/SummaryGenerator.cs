namespace ProductLeaders.console.Utilities;

public static class SummaryGenerator
{
    public static void SaveSummary(string filePath, List<ServiceHighlight> highlights)
    {
        var jsonContent = JsonSerializer.Serialize(highlights, new JsonSerializerOptions
        {
            WriteIndented = true // Pretty-print JSON
        });

        File.WriteAllText(filePath, jsonContent);
    }

    ///
    public static List<ServiceHighlight> GenerateSummary(string service_path)
{
    var filePath = service_path;
    var config = new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
    {
        HasHeaderRecord = true,
    };
    using (var reader = new StreamReader(filePath))
    using (var csv = new CsvReader(reader, config))
    {
        var records = csv.GetRecords<CSVFeedbackRecord>().ToList();

        // Group by ServiceTree_Name and calculate metrics
        var serviceHighlights = records
            .GroupBy(r => r.ServiceName)
            .Select(g =>
            {
                var totalFeedback = g.Count();
                var feedbackByType = g.GroupBy(r => r.Type)
                                    .Select(t => new FeedbackTypeSummary
                                    {
                                        Type = t.Key,
                                        Count = t.Count()
                                    })
                                    .ToList();

                // Separate out the feedback types below 5%
                var collapsedTypes = feedbackByType
                    .Where(ft => ft.Count < totalFeedback * 0.05) // Below 5%
                    .Select(ft => new FeedbackTypeDetail { OriginalType = ft.Type, Count = ft.Count })
                    .ToList();

                // Filter out the ones that are above 5%
                var feedbackSummary = feedbackByType
                    .Where(ft => ft.Count >= totalFeedback * 0.05)
                    .ToList();

                // Add "Other" category with collapsed types
                if (collapsedTypes.Any())
                {
                    feedbackSummary.Add(new FeedbackTypeSummary
                    {
                        Type = "Other",
                        Count = collapsedTypes.Sum(ct => ct.Count),
                        Details = collapsedTypes
                    });
                }

                return new ServiceHighlight
                {
                    ServiceName = g.Key,
                    TotalFeedback = totalFeedback,
                    DistinctCustomers = g.Select(r => r.CustomerTpid).Distinct().Count(),
                    FeatureRequests = g.Count(r => r.Type == "Feature Request"),
                    Bugs = g.Count(r => r.Type == "Bug"),
                    OverallSentiment = g.Count(r => r.Type == "Feature Request") > g.Count(r => r.Type == "Bug") ? "Positive" : "Neutral",
                    FeedbackTypes = feedbackSummary  // Include the feedback summary with "Other"
                };
            })
            .ToList();

        return serviceHighlights;  // Return the list of ServiceHighlight objects
    }
}
    ///
}
