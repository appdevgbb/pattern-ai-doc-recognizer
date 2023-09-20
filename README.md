# Pattern - Using Azure OpenAI and AI Document Intelligence

A pattern that showcases how to use AI Document Intelligence and document summarization  
  
### Topology:  
  
- [x] AI Document Intelligence  
- [x] Azure OpenAI  
- [x] Azure Functions  
  
### Steps to run this demo  
  
You have two options for using this demo: you can either set it up locally on your machine or use GitHub Codespaces with devcontainers.  
  
### Option 1: Local Development  
  
You can develop this demo locally on your own machine. Here's a simple guide to help you get started:  
  
1. Clone the repository to your local machine.  
2. Open the repository in your preferred code editor.  
3. Ensure that you have the necessary software and libraries installed. You will need the [.NET 7 SDK](https://learn.microsoft.com/en-us/dotnet/core/tools/), [Azure Functions Core Tools](https://github.com/Azure/azure-functions-core-tools/blob/v4.x/README.md#linux), and [Azure CLI](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli).  
4. Build and test the application locally using Azure Functions.
  
### Option 2: GitHub Codespaces with devcontainers  
  
Alternatively, you can use GitHub Codespaces with devcontainers. This method allows you to use a pre-configured development environment and avoids the need to install anything on your local machine.  
  
Here's a simple guide to get you started:  
  
1. Navigate to the repository on GitHub.  
2. Click on the 'Code' button and then select 'Open with Codespaces'.  
3. Click on the '+ New codespace' button.  
4. After a few moments, your codespace should be ready and you'll be taken to an online version of Visual Studio Code.  
  
This codespace is a full-fledged development environment and you can write and run your code just like you would on your local machine. The devcontainer configuration in the repository sets up all the necessary software and libraries for you.  

#### Creating an Azure OpenAI instance using gpt-3.5-turbo  
  
Currently, Azure CLI doesn't support creating an Azure OpenAI instance directly. You'll need to use the OpenAI API.   
  
To interact with OpenAI GPT-3.5 Turbo, sign up on the OpenAI website, get your API keys, and use them in your application following the [OpenAI API documentation](https://beta.openai.com/docs/).  
  
#### Creating an AI Document Intelligence instance  
  
As of now, Azure CLI does not support the direct creation of AI Document Intelligence resources.   
  
You can create and manage AI Document Intelligence resources using the Azure portal, REST APIs, or SDKs. Please refer to the [official Azure AI Document Intelligence documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/document-intelligence-service/) for detailed steps.  

 #### Setting up the environment

First, setup your environment variables

```bash
export SUBSCRIPTION_ID=VALUE  
export RESOURCE_GROUP_NAME=VALUE  
export LOCATION=VALUE  
export STORAGE_ACCOUNT_NAME=VALUE  
export FUNCTION_APP_NAME=VALUE  
```

#### Creating an Azure Function with dotnet-isolated  
  
Login to Azure and set your subscription:  
  
```bash  
az login  
az account set --subscription ${SUBSCRIPTION_ID}  
```
  
Next, create a resource group, a storage account, and an Azure Functions app:  
  
```bash  
az group create --name ${RESOURCE_GROUP_NAME} --location $LOCATION

az storage account create \
 --name ${STORAGE_ACCOUNT_NAME} \
 --location ${LOCATION} \
 --resource-group ${RESOURCE_GROUP_NAME} \
 --sku Standard_LRS  

az functionapp create \
 --resource-group ${RESOURCE_GROUP_NAME} \
 --consumption-plan-location ${LOCATION} \
 --runtime dotnet-isolated \
 --functions-version 4 \
 --name ${FUNCTION_APP_NAME} \
 --storage-account ${STORAGE_ACCOUNT_NAME}
```

#### Deploy Your Code to Azure Functions

Now you're ready to deploy your code to Azure Functions. You can use the Azure Function CLI for this.   
  
```bash  
func azure functionapp publish ${FUNCTION_APP_NAME}
```
  
This will upload the contents of the `publish` directory to your Function App in Azure.

### Removing the resources

To remove the Azure Functions, run the following command:

```bash
az group delete --name ${RESOURCE_GROUP_NAME}
```
