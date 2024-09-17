public class SummaryDetail
{
    [JsonPropertyName("main_points")]
    public List<MainPoint> MainPoints { get; set; } = new List<MainPoint>();
}

public class MainPoint
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public List<string> Description { get; set; } = new List<string>();
}
