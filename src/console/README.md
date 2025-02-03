# Product Leader Copilot CLI (fbl)

The **Product Leader Copilot CLI** (also known as **fbl**) is a command-line tool that helps you generate summaries, user story JSON, clusters, and perform vector searches on feedback data.

> **Note:** This tool is part of the [Product Leader Copilot](https://github.com/yodobrin/feedback-ai-lens) project.

## Features

- **Summary:** Generate a feedback summary from an input CSV file.
- **JSON:** Create user story representations using OpenAI.
- **Cluster:** Cluster feedback based on embedding similarities.
- **Search:** Perform vector-based searches with customizable queries.
  The search operation supports two modes:
  - `find_customers`
  - `find_use_cases`

## Installation

### Option 1: Download the Executable

1. Go to the [GitHub Releases](https://github.com/yodobrin/feedback-ai-lens/releases) page.
2. Download the appropriate executable for your platform (e.g., `fbl` for macOS).
3. (Optional) Place the executable in a folder that’s in your `PATH` (e.g., `/usr/local/bin`).

### Option 2: Install as a .NET Global Tool

If you prefer, you can package and distribute this CLI tool as a .NET global tool. (See the [GitHub Actions workflow](.github/workflows/release.yml) for packaging instructions.)

```bash
dotnet tool install -g ProductLeaderCopilotCLI --version <version>
```

Then, run it using:

```bash
fbl <RunConfigFile> <EnvFile>
```

## Usage

The CLI tool requires two arguments:
	•	RunConfigFile: A JSON file that defines the run configuration.
	•	EnvFile: A file containing the required environment variables.

Example Run Configuration (run.json)

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

Example Environment File (.env)

```env
AOAI_APIKEY=your_api_key_here
AOAI_ENDPOINT=https://your-endpoint.azure.com/
CHATCOMPLETION_DEPLOYMENTNAME=your_chat_deployment
EMBEDDING_DEPLOYMENTNAME=your_embedding_deployment
```

### Running the Tool

After installing or downloading the executable, run it as follows:

```bash
# On macOS or Linux:
fbl ./run.json ../../configuration/.env

# On Windows (if using the .exe version):
fbl-win-x64.exe ./run.json ..\..\configuration\.env
```

## GitHub Actions Workflow

This repository includes a GitHub Actions workflow that builds self-contained executables for multiple platforms:
	•	osx-x64 (macOS Intel)
	•	osx-arm64 (macOS Apple Silicon)
	•	win-x64 (Windows)

The workflow is defined in .github/workflows/release.yml and uses a matrix strategy to build the binaries. When you push a version tag (e.g., v1.0.0), the workflow automatically publishes the executables as artifacts. You can then download these artifacts or attach them to a GitHub release.

## Help

Run the tool without any arguments or with --help/-h to display usage instructions.

## Contributing

Contributions are welcome! Feel free to open issues or submit pull requests.

## License

This project is licensed under the MIT License.
