using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class Settings
{
    [ConfigurationKeyName("AZURE_OPENAI_ENDPOINT")]    
    public string AzureOpenAIEndpoint {get; set;}

    [ConfigurationKeyName("AZURE_OPENAI_EMBEDDING_DEPLOYMENT")]
    public string AzureOpenAIEmbeddingDeployment {get; set;}

    [ConfigurationKeyName("AZURE_OPENAI_CHAT_DEPLOYMENT")]
    public string AzureOpenAIChatDeployment {get; set;}

    [ConfigurationKeyName("AZURE_DOCUMENT_INTELLIGENCE_ENDPOINT")]
    public string AzureDocumentIntelligenceEndpoint {get; set;}

    [ConfigurationKeyName("AZURE_AI_SEARCH_ENDPOINT")]
    public string AzureAISearchEndpoint {get; set;}

    [ConfigurationKeyName("AZURE_STORAGE_ACCOUNT_ENDPOINT")]
    public string AzureStorageAccountEndpoint {get; set;}
}