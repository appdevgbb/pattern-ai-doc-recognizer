using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

namespace pattern_ai_doc_recognizer
{
    public class SummarizeText
    {
        private readonly ILogger _logger;
        private readonly IKernel _kernel;

        public SummarizeText(ILoggerFactory loggerFactory, IKernel kernel)
        {
            _logger = loggerFactory.CreateLogger<SummarizeText>();
            _kernel = kernel;
        }

        [Function("SummarizeText")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] 
            HttpRequestData req,
            FunctionContext executionContext)
        {
            _logger.LogInformation("SummarizeText function triggered.");

            // Read text and Id from request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var requestBodyJson = JsonDocument.Parse(requestBody);
            var text = requestBodyJson.RootElement.GetProperty("text").GetString();
            var id = requestBodyJson.RootElement.GetProperty("id").GetString();        

            // Find the plugins folder and import the semantic skill            
            var pluginDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Plugins");
            var plugin = _kernel.ImportSemanticSkillFromDirectory(pluginDirectory, "SemanticPlugin");

            // A semantic skill is a collection of functions 
            var functionName = "Summarize";
            var function = plugin[functionName];

            // Run the function and pass in the input variable
            var context = new ContextVariables();
            context.Set("input", text);
            var result = await _kernel.RunAsync(context, function);

            // Return the result
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            response.WriteString(result.ToString());

            return response;
        }
    }
}
