# Start from the universal base image  
FROM mcr.microsoft.com/vscode/devcontainers/universal:2  
  
# Install Azure Functions Core Tools  
RUN wget -q https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb && \  
    dpkg -i packages-microsoft-prod.deb && \  
    apt-get update && \  
    apt-get install -y azure-functions-core-tools-4
