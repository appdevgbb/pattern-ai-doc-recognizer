using System.Net;
using System.Text.Json;
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using DocumentSummarization.Models;

namespace DocumentSummarization
{
    public class ProcessFile
    {
        private readonly ILogger _logger;
        private readonly DocumentAnalysisClient _documentAnalysisClient;
        private readonly IKernel _kernel;

        public ProcessFile(ILoggerFactory loggerFactory, DocumentAnalysisClient documentAnalysisClient, IKernel kernel)
        {
            _logger = loggerFactory.CreateLogger<ProcessFile>();
            _documentAnalysisClient = documentAnalysisClient;
            _kernel = kernel;
        }

        [Function("ProcessFile")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] 
            HttpRequestData req,
            FunctionContext executionContext)
        {
            _logger.LogInformation("ProcessFile function triggered.");

            // Read fileUri and Id from request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var requestBodyJson = JsonDocument.Parse(requestBody);
            var fileUri = requestBodyJson.RootElement.GetProperty("fileUri").GetString();
            var id = requestBodyJson.RootElement.GetProperty("id").GetString();        
            
            // Check for missing fileUri property
            if (fileUri == null)
            {
                throw new ArgumentNullException("fileUri is null. Please check your request body.");
            }

            // Analyze the file with the document intelligence service (artist formerly known as Form Recognizer)
            // string modelId = "prebuilt-document";
            string modelId = "model1";
            AnalyzeDocumentOperation operation = await _documentAnalysisClient.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, modelId, new Uri(fileUri));
            AnalyzeResult analyzeResult = operation.Value;     

            // Iterate over the documents and return a list of fields for the custom model
            var fields = new List<CustomField>();
            foreach (var document in analyzeResult.Documents)
            {
                
                foreach (var field in document.Fields)
                {
                    fields.Add(new CustomField(field.Key, field.Value.Content));                    
                }
            }    

            // Find the plugins folder and import the semantic skill            
            var pluginDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Plugins");
            var plugin = _kernel.ImportSemanticSkillFromDirectory(pluginDirectory, "SemanticPlugin");

            // A semantic skill is a collection of functions 
            var functionName = "Summarize";
            var function = plugin[functionName];

            // Run the function and pass in the input variable
            var context = new ContextVariables();
            context.Set("input", fields[0].Content);
            var result = await _kernel.RunAsync(context, function);

            // Return the result
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            response.WriteString(result.ToString());

            return response;
        }
    }
}
