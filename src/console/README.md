
# Product Leader Copilot CLI

The Product Leader Copilot CLI is a command-line tool designed to help you generate summaries, create user story JSON representations, cluster feedback, and perform vector-based searches on feedback data. This tool is part of the Product Leader Copilot project and is intended to be as simple as possible—even non-programmers can use it.

## Prerequisites

Before using the CLI tool, ensure that you have the following installed:
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (or later)

## Getting Started

You have two simple options to run the tool:

**Option 1**: Run Directly with dotnet run

If you just want to try the tool without creating a dedicated executable, you can run it directly from the source. From the repository root, open a terminal and execute:

```bash
dotnet run --project src/console/console.csproj <RunConfigFile> <EnvFile>
```

Replace <RunConfigFile> with your run configuration JSON file (for example, run.json) and <EnvFile> with your environment file (for example, .env). This command uses the current platform as the target.

**Option 2**: Build a Self-Contained Executable

If you prefer to build a standalone executable for your platform, you can publish the tool with a specific runtime identifier. The default target is the one you’re running on, but here are some common options:

- macOS (Intel): osx-x64
- macOS (Apple Silicon): osx-arm64
- Windows (x64): win-x64

To publish for your platform, run a command like:

```bash
dotnet publish src/console/console.csproj -c Release -r <RID> --self-contained true -o ./publish
```

Replace <RID> with the runtime identifier for your platform (for example, osx-arm64 on Apple Silicon). The resulting executable will be located in the ./publish folder.

Once built, you can run the executable like this:

On macOS/Linux:

```bash
./publish/console <RunConfigFile> <EnvFile>
```

On Windows (if the executable name ends with .exe):

```bash
publish\console.exe <RunConfigFile> <EnvFile>
```


## Run Configuration and Environment Files

The CLI tool requires two files to run:
	1.	RunConfigFile: A JSON file that defines the run configuration.
	2.	EnvFile: A file that contains the required environment variables.

Example Run Configuration (`run.json`)
```json
{
  "service": "adf",
  "input_file": "../../sample-data/adf.csv",
  "data_folder": "../../sample-data/",
  "output_directory": "results/",
  "operation": "search",
  "parameters": {
    "service_mapping_file": "service_mapping.json",
    "embedding_dimension": 1536,
    "similarity_threshold": 0.75,
    "search_query": "need for improved security measures and streamlined authentication processes across various role",
    "query_type": "find_customers"
  }
}
```

Example Environment File (`.env`)

```plaintext
AOAI_APIKEY=your_api_key_here
AOAI_ENDPOINT=https://your-endpoint.azure.com/
CHATCOMPLETION_DEPLOYMENTNAME=your_chat_deployment
EMBEDDING_DEPLOYMENTNAME=your_embedding_deployment
```
## Usage

After setting up your configuration and environment files, you can run the CLI tool as follows:
	•	Using dotnet run:

```bash
dotnet run --project src/console/console.csproj run.json .env
```

Using a Published Executable:
For example, after publishing for your platform (e.g., osx-arm64):

```bash
./publish/console run.json .env
```


The tool also supports a help option. Running the tool without any arguments or with --help or -h will display usage instructions.

## Summary
- For non-programmers: The easiest way is to use dotnet run as shown above.
- For those who want a dedicated executable: Use the dotnet publish command with your platform’s runtime identifier.
- Common Runtime Identifiers:
  - macOS (Intel): osx-x64
  - macOS (Apple Silicon): osx-arm64
  - Windows (x64): win-x64

## Contributing

Contributions are welcome! Feel free to open issues or submit pull requests if you have suggestions or improvements.

## License

This project is licensed under the MIT License.
