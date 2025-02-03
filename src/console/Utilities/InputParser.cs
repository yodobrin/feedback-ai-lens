namespace ProductLeaders.console.Utilities;


public static class InputParser
{
    /// <summary>
    /// Reads and parses a JSON input file into a RunConfig object.
    /// </summary>
    /// <param name="filePath">Path to the input JSON file.</param>
    /// <returns>A RunConfig object representing the parsed data.</returns>
    public static RunConfig Parse(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Input file not found at: {filePath}");
        }

        try
        {
            string jsonContent = File.ReadAllText(filePath);
            RunConfig? input = JsonSerializer.Deserialize<RunConfig>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // Allows case-insensitive property matching
            });

            if (input == null)
            {
                throw new InvalidOperationException("The input file is empty or contains invalid JSON.");
            }

            Validate(input);

            return input;
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Failed to parse the input file. Ensure it is valid JSON.", ex);
        }
    }

    /// <summary>
    /// Validates the RunConfig object to ensure required fields are present.
    /// </summary>
    /// <param name="input">The RunConfig object to validate.</param>
    private static void Validate(RunConfig input)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input), "Input data cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(input.InputFile))
        {
            throw new InvalidOperationException("Input file must be specified.");
        }

        if (string.IsNullOrWhiteSpace(input.OutputDirectory))
        {
            throw new InvalidOperationException("Output directory must be specified.");
        }

        if (string.IsNullOrWhiteSpace(input.Operation))
        {
            throw new InvalidOperationException("Operation type must be specified.");
        }
    }
}
