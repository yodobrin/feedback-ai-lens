// namespace ProductLeaders.console.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
public class ClassificationNode
{
    [JsonPropertyName("topic")]
    public string Topic { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("definition")]
    public string Definition { get; set; } = string.Empty;

    [JsonPropertyName("embedded")]
    public float[] Embedded { get; set; } = Array.Empty<float>();

    // Child topics to allow deeper nesting (e.g., subcategories)
    [JsonPropertyName("child_topics")]
    public List<ClassificationNode> ChildTopics { get; set; } = new List<ClassificationNode>();

    /// <summary>
    /// Recursively builds a list of "paths" to each leaf node.
    /// E.g., "Reliability::Simplicity and efficiency"
    /// You could also change the return to void and do something else with the path.
    /// </summary>
    public List<string> GetAllClassificationKeys(string ? parentPath = null, string separator = "::")
    {
        // Build the current node's path
        string currentPath = string.IsNullOrWhiteSpace(parentPath)
            ? Topic
            : $"{parentPath}{separator}{Topic}";

        var results = new List<string>();

        // If this node has no children, it's a leaf => add the path
        if (ChildTopics == null || ChildTopics.Count == 0)
        {
            results.Add(currentPath);
        }
        else
        {
            // If it has children, gather their paths
            foreach (var child in ChildTopics)
            {
                results.AddRange(child.GetAllClassificationKeys(currentPath, separator));
            }
        }

        return results;
    }

    /// <summary>
    /// (Optional) If you want a single "key" for this node specifically,
    /// ignoring children, you could do something like:
    /// </summary>
    public string GetClassificationKey(string ? parentPath = null, string separator = "::")
    {
        if (string.IsNullOrWhiteSpace(parentPath)) return Topic;
        return $"{parentPath}{separator}{Topic}";
    }
}
