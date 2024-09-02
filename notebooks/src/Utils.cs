using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using dotenv.net;
using Azure;
using Azure.Identity;
using Azure.AI.DocumentIntelligence;

public static class Utils
{
    public static Settings LoadConfig(string configPath)
    {
        Console.WriteLine($"Loading config from {configPath}");

        DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] {configPath}));
        var env = DotEnv.Read();

        var builder = new ConfigurationBuilder().AddInMemoryCollection(env);
        var configuration = builder.Build();

        var settings = new Settings();

        configuration.Bind(settings);


        return settings;
    }

    public static DocumentIntelligenceClient SetupDocumentIntelligence(string endpoint)
    {
        var documentIntelligenceClient = new DocumentIntelligenceClient(new Uri(endpoint), new DefaultAzureCredential());
        return documentIntelligenceClient;
    }

    public static MemoryServerless SetupMemoryServerless(Settings settings)
    {
        var endpoint = settings.AzureOpenAIEndpoint;
        var config = new AzureOpenAIConfig()
        {
            APIType = AzureOpenAIConfig.APITypes.ChatCompletion,
            Auth = AzureOpenAIConfig.AuthTypes.AzureIdentity,
            Endpoint = endpoint,
            Deployment = settings.AzureOpenAIChatDeployment,
        };
        var embeddingConfig = new AzureOpenAIConfig()
        {
            APIType = AzureOpenAIConfig.APITypes.EmbeddingGeneration,
            Auth = AzureOpenAIConfig.AuthTypes.AzureIdentity,
            Endpoint = endpoint,
            Deployment = settings.AzureOpenAIEmbeddingDeployment,
        };
        var searchClientConfig = new SearchClientConfig()
        {
            AnswerTokens = 4096
        };

        var memory = new KernelMemoryBuilder()
            .WithAzureOpenAITextGeneration(config)
            .WithAzureOpenAITextEmbeddingGeneration(embeddingConfig)
            .WithSearchClientConfig(searchClientConfig)
            .Build<MemoryServerless>();

        return memory;
    }
}