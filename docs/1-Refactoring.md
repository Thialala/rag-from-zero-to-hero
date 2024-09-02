# Refactoring and Extraction of Logic into C# Classes

## Overview

This part focuses on the refactored logic (first part) from the first two Jupyter Notebooks that was used to process OCR tasks and manage document workflows. The original code has been modularized and extracted into separate C# classes to enhance maintainability and reusability, for future Notebooks. This README will explain the purpose of each class, how they interact with one another, and how to use them.

## Classes and Structure

### 1. **Settings.cs**
   - **Purpose**: 
     - This class is responsible for managing configuration settings used throughout the project. It loads the necessary endpoints and configurations required for interacting with Azure services.
   - **Key Properties**:
     - `AzureOpenAIEndpoint`: The endpoint for Azure OpenAI.
     - `AzureOpenAIEmbeddingDeployment`: Deployment configuration for embedding services.
     - `AzureOpenAIChatDeployment`: Deployment configuration for chat services.
     - `AzureDocumentIntelligenceEndpoint`: The endpoint for Azure Document Intelligence.
     - `AzureAISearchEndpoint`: The endpoint for Azure AI Search.
     - `AzureStorageAccountEndpoint`: The endpoint for Azure Storage Account.
   - **Usage**: Instantiate this class to load and access configuration settings from the environment variables or configuration files.

### 2. **Utils.cs**
   - **Purpose**: 
     - This static utility class provides methods for setting up necessary services and loading configurations.
   - **Key Methods**:
     - `LoadConfig(string configPath)`: Loads environment variables from a specified file and binds them to a `Settings` object.
     - `SetupDocumentIntelligence(string endpoint)`: Initializes the `DocumentIntelligenceClient` used for OCR processing.
     - `SetupMemoryServerless(Settings settings)`: Configures and returns a `MemoryServerless` instance for managing memory and search operations.
   - **Usage**: Use these utility functions to initialize the necessary services before running workflows.

### 3. **RAGWorkflow.cs**
   - **Purpose**: 
     - This class encapsulates the main workflow logic, including running OCR on PDF files to convert them in Markdown and importing Markdown files into memory for later querying.
   - **Key Methods**:
     - `RunAsync(string folderPath)`: Orchestrates the OCR processing and Markdown file import tasks within a specified folder.
     - `OCROnPDFFilesAsync(string folderPath)`: Performs OCR on PDF files within the specified folder and saves the results as Markdown files.
     - `ImportMarkdownFilesAsync(string folderPath)`: Imports the generated Markdown files into a memory server for later querying.
     - `AskQuestionAsync(string question)`: Allows querying the memory for information based on the imported documents.
   - **Usage**: Instantiate this class with initialized clients (e.g., `DocumentIntelligenceClient`, `MemoryServerless`) and use its methods to run OCR workflows and query the processed data.

## Example Workflow

To utilize this refactored setup, follow these steps:

1. **Load Configuration**:
   ```csharp
   var configPath = "path/to/your/.env";
   var settings = Utils.LoadConfig(configPath);
   ```

2. **Initialize Services**:
   ```csharp
   var documentClient = Utils.SetupDocumentIntelligence(settings.AzureDocumentIntelligenceEndpoint);
   var memory = Utils.SetupMemoryServerless(settings);
   ```

3. **Run Workflow**:
   ```csharp
   var workflow = new RAGWorkflow(documentClient, memory);
   await workflow.RunAsync("path/to/your/documents");
   ```

4. **Ask Questions**:
   ```csharp
   await workflow.AskQuestionAsync("What is the summary of document X?");
   ```

## Conclusion

This refactoring separates concerns and encapsulates logic into reusable and modular classes. This approach improves the maintainability of the codebase and makes it easier to extend and adapt to future needs. The modular design allows each component to be tested independently and reused across different projects or notebooks.