using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.Text.Json;


public static class Program
{
    public static void Main()
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureAppConfiguration(configuration =>
            {
                var config = configuration.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);

                var builtConfig = config.Build();
            })
            .ConfigureServices(services =>
            {        
                var documentAnalysisEndpoint = Environment.GetEnvironmentVariable("DocumentAnalysisEndpoint");
                var documentAnalysisKey = Environment.GetEnvironmentVariable("DocumentAnalysisKey");

                // Check for null values for documentAnalysisEndpoint and documentAnalysisKey
                if (documentAnalysisEndpoint == null || documentAnalysisKey == null)
                {
                    throw new ArgumentNullException("DocumentAnalysisEndpoint or DocumentAnalysisKey is null. Please check your local.settings.json file.");
                }

                // Add document analysis client and kernel to DI container
                services.AddSingleton<DocumentAnalysisClient>(new DocumentAnalysisClient(new Uri(documentAnalysisEndpoint), new AzureKeyCredential(documentAnalysisKey)));
                services.AddTransient((provider) => CreateKernel(provider));

                // Return JSON with expected lowercase naming
                services.Configure<JsonSerializerOptions>(options =>
                {
                    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });
            })
            .Build();

        host.Run();
    }

    private static IKernel CreateKernel(IServiceProvider provider)
    {
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .SetMinimumLevel(LogLevel.Warning)
                .AddConsole()
                .AddDebug();
        });

        // Get OpenAI settings from environment variables
        var azureOpenAiChatDeploymentName = Environment.GetEnvironmentVariable("AzureOpenAiChatDeploymentName");
        var azureOpenAiEndpoint = Environment.GetEnvironmentVariable("AzureOpenAiEndpoint");
        var azureOpenAiKey = Environment.GetEnvironmentVariable("AzureOpenAiKey");

        // Check to see that the environment variables are not null
        if (azureOpenAiChatDeploymentName == null || azureOpenAiEndpoint == null || azureOpenAiKey == null)
        {
            throw new ArgumentNullException("AzureOpenAiChatDeploymentName, AzureOpenAiEndpoint, or AzureOpenAiKey is null. Please check your local.settings.json file.");
        }

        var kernel = new KernelBuilder()
            .WithAzureChatCompletionService(
                azureOpenAiChatDeploymentName, 
                azureOpenAiEndpoint, 
                azureOpenAiKey)
            .Build();

        return kernel;   
    }
}