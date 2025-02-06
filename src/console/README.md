
# Product Leader Copilot CLI

The Product Leader Copilot CLI is a command-line tool designed to help you generate summaries, create user story JSON representations, cluster feedback, and perform vector-based searches on feedback data. This tool is part of the Product Leader Copilot project and is intended to be as simple as possible—even non-programmers can use it.

## Prerequisites

Before using the CLI tool, ensure that you have the following installed:
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (or later)

## Getting Started

You have two simple options to run the tool:

Lets focus on running directly with `dotnet run`

First, you will need to clone the repository:

```bash
git clone https://github.com/yodobrin/feedback-ai-lens
```


Then navigate to the console project:

```bash
cd feedback-ai-lens/src/console
```

The actual command to run the tool is:

```bash
dotnet run  <RunConfigFile> <EnvFile>
```

Replace <RunConfigFile> with your run configuration JSON file (for example, run.json) and <EnvFile> with your environment file (for example, .env). This command uses the current platform as the target.

> Note: The tool will not run without the required files and input files and output folders - obtain/create them before running the tool.

## Run Configuration and Environment Files

The CLI tool requires two files to run:
	1.	RunConfigFile: A JSON file that defines the run configuration, what is it we aim to run.
	2.	EnvFile: A file that contains the required environment variables. (OpenAI API Key, Azure Endpoint, etc.)

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

## Generating insight from Feedbacks

### High Level Overview of the process

- The process start by harvesting feedback data from the specific ADX cluster.
- Generating user stories and embedding
- Creating clusters of feedbacks

### Harvesting Feedback Data

The query used is:

```kql
Feedback
| where ServiceTree.Name in ('App Service (Web Apps)', 'Azure VMware Solution', 'Azure Kubernetes Service')
| summarize arg_max(PartnerReceivedDate, *) by Id
| extend CleanDescription = replace_regex(Description, @"<[^>]*>", '') // Remove HTML tags from Description
| extend CleanWorkaroundDescription = replace_regex(WorkaroundDescription, @"<[^>]*>", '') // Remove HTML tags from WorkaroundDescription
| project Id, PartnerShortName, ServiceTree.Name, Type, Title, Blocking, CleanDescription, WorkaroundAvailable, Priority, Customer.Name,Customer.Tpid, CleanWorkaroundDescription
```
> Note: This query is a sample of how to get feedback for App Service (Web Apps), Azure VMware Solution, Azure Kubernetes Service

The output is assume to be a csv file, the file **must** match the projected columns in the query, in the case an alteration is required, the matching .NET class should be updated `CSVFeedbackRecord.cs`.

> Note: Without a csv file of feedbacks, you cannot use the tool. Reach out to the author for a sample data. You will need to create the folders `sample-data` and place the csv file in it, and `/results` for the output.

### Running specific operations

Per operation:
- `summary`   - Generate a summary from the input file.
- `json`      - Generate user stories from feedback using OpenAI.
- `cluster`   - Cluster feedback records based on their embeddings.
- `search`    - Perform a vector search using embeddings. Requires additional parameters:
  - `search_query`: The query string for searching.
  - `query_type`: Either 'find_customers' or 'find_use_cases'.

Each of these operations runs independently and can be run separately. There is however dependencies between them, for example, `json` operation must be run before `cluster` and `search` operations.

## Contributing

Contributions are welcome! Feel free to open issues or submit pull requests if you have suggestions or improvements.

## License

This project is licensed under the MIT License.
