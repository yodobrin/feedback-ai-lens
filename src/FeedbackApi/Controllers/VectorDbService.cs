// using VectorLibrary;
// using Azure.Storage.Blobs;
using System.IO;
using Azure.AI.OpenAI;
using Azure;
using Azure.Core;


    public class VectorDbService
    {
        public VectorCollection ? VectorCollection { get; private set; }
        private OpenAIClient ? _openAIClient;
        private string ? _embeddingDeploymentName;

        public VectorDbService()
        {
            Console.WriteLine("VectorDbService constructor called");            
        }
        public async Task<SearchResult> SearchByDotProduct(string query)
        {
            // check the vector collection is not null throw exception
            if (VectorCollection == null)
            {
                throw new Exception("VectorCollection is null");
            }
            var queryVector = await GetEmbeddings(query);
            return VectorCollection.FindByDotProduct(queryVector, item => item.GetVector());
        }            
        public async Task<SearchResult> SearchByCosineSimilarity(string query)
        {
            // check the vector collection is not null throw exception
            if (VectorCollection == null)
            {
                throw new Exception("VectorCollection is null");
            }
            var queryVector = await GetEmbeddings(query);
            return VectorCollection.FindByCosineSimilarity(queryVector, item => item.GetVector());
        }
        public async Task<SearchResult> SearchByEuclideanDistance(string query)
        {
            // check the vector collection is not null throw exception
            if (VectorCollection == null)
            {
                throw new Exception("VectorCollection is null");
            }
            var queryVector = await GetEmbeddings(query);
            return VectorCollection.FindByEuclideanDistance(queryVector, item => item.GetVector());
        }
        // public async Task CreateDatabaseAsync(string inputCsvFileName, string outputJsonFileName)
        // {
        //     // check for null on embeddingDeploymentName & openAIClient throw exception
        //     if (string.IsNullOrEmpty(_embeddingDeploymentName)  || _openAIClient == null)
        //     {
        //         throw new Exception("OpenAI Client or Embedding Deployment Name is null");
        //     }
        //     List<FunctionCodePair> functionCodePairs = await StoreUtility.LoadFunctionCodePairsFromAzureBlobAsync(inputCsvFileName,_openAIClient,_embeddingDeploymentName);
        //     await StoreUtility.SaveFunctionCodePairsToAzureBlobAsync(functionCodePairs, outputJsonFileName);

        // }

        // public async Task SaveFunctionCodePairsToBlobAsync(string jsonFileName)
        // {
        //     if (VectorCollection == null)
        //     {
        //         throw new InvalidOperationException("VectorCollection is not initialized.");
        //     }
        //     // Call the method to save the function code pairs to Azure Blob
        //     await StoreUtility.SaveFunctionCodePairsToAzureBlobAsync(VectorCollection.GetFunctionCodePairs(), jsonFileName);
        // }

        private async Task<float[]> GetEmbeddings(string query)
        {
            // null check for embeddingDeploymentName & openAIClient throw exception
            if (_embeddingDeploymentName == null || _openAIClient == null)
            {
                throw new Exception("OpenAI Client or Embedding Deployment Name is null");
            }

            EmbeddingsOptions embeddingsOptions = new EmbeddingsOptions(_embeddingDeploymentName,new List<string> { query });
            var embeddingsResponse = await _openAIClient.GetEmbeddingsAsync(embeddingsOptions);
            return embeddingsResponse.Value.Data[0].Embedding.ToArray();
        }
        
    private async Task LoadDataFromLocalFolder(string jsonFileName)
    {
        // Get the path to the local folder (you can customize this)
        string localFolderPath = Environment.GetEnvironmentVariable("LOCAL_FOLDER_PATH") ?? "LOCAL_FOLDER_PATH not found";

        // Check if the environment variable is set correctly
        if (localFolderPath == "LOCAL_FOLDER_PATH not found" || string.IsNullOrEmpty(jsonFileName))
        {
            Console.WriteLine("One or more environment variables are not set. Please set LOCAL_FOLDER_PATH and ensure jsonFileName is not empty.");
            return;
        }

        // Construct the full file path
        string fullFilePath = Path.Combine(localFolderPath, jsonFileName);

        // Check if the file exists
        if (File.Exists(fullFilePath))
        {
            try
            {
                // Read the file asynchronously
                using (FileStream fileStream = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read))
                {
                    // Pass the stream to your existing VectorCollection logic
                    VectorCollection = await VectorCollection.CreateFromMemoryAsync(fileStream);
                }

                Console.WriteLine("File loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while reading the file: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine($"File does not exist: {fullFilePath}");
        }
    }

        public async Task InitializeAsync(string jsonFileName)
        {
            Console.WriteLine("Initializing VectorDbService & OpenAI Client");
            // await LoadDataFromBlobStorage(jsonFileName);
            await LoadDataFromLocalFolder(jsonFileName);
            string oAiApiKey = Environment.GetEnvironmentVariable("AOAI_APIKEY") ?? "AOAI_APIKEY not found";
            string oAiEndpoint = Environment.GetEnvironmentVariable("AOAI_ENDPOINT") ?? "AOAI_ENDPOINT not found";
            _embeddingDeploymentName = Environment.GetEnvironmentVariable("EMBEDDING_DEPLOYMENTNAME") ?? "EMBEDDING_DEPLOYMENTNAME not found";

            AzureKeyCredential azureKeyCredential = new AzureKeyCredential(oAiApiKey);
            _openAIClient = new OpenAIClient(new Uri(oAiEndpoint), azureKeyCredential);
            Console.WriteLine("... Initialized VectorDbService & OpenAI Client !");
        }
    }
