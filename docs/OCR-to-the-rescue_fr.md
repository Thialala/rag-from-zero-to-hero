Amélioration du RAG avec Azure Document Intelligence et le format Markdown
====================================================================================================

Introduction
------------

Dans la [partie précédente](../notebooks/0-getting-started.ipynb), nous avons constaté une limitation majeure lors de la mise en œuvre du RAG en effectuant un split direct du PDF : les tableaux ne sont pas pris en compte (voir captures d'écran ci-dessous). Par conséquent, il est impossible pour le modèle de répondre aux questions qui s'y réfèrent.

![Tableau non pris en compte](images/00_cost_per_employee.png)
![Pas de réponse du système RAG](images/01_not_answered_question_cost_per_employee.png)

1. Solution : Azure Document Intelligence
----------------------------------------

Pour résoudre ce problème, nous avons introduit [Azure Document Intelligence](https://learn.microsoft.com/en-us/azure/ai-services/document-intelligence/overview?view=doc-intel-4.0.0), un service d'intelligence artificielle qui effectue l'OCR des documents PDF et extrait le contenu sous forme de texte structuré. Ce service permet non seulement d'extraire les tableaux, mais aussi de conserver la structure et la sémantique du document d'origine.

L'IaC a été mis à jour pour inclure la ressource Azure Document Intelligence:
- nouveau module : [document-intelligence.bicep](../infra/core/ai/doc-intelligence.bicep)
- mise à jour du [main.bicep](../infra/main.bicep) pour inclure le module.

2. Conversion en format Markdown
-------------------------------

Azure Document Intelligence convertit le contenu des PDF en format Markdown, un langage qui permet de structurer le texte et d'ajouter de la sémantique. Cette conversion offre plusieurs avantages :

a. Prise en compte des tableaux : Les tableaux sont correctement extraits et convertis en format Markdown, ce qui permet au modèle RAG de les traiter et d'y répondre.

b. Sémantique améliorée : La structure du Markdown fournit une sémantique supplémentaire au modèle de langage (LLM), facilitant ainsi la compréhension du contexte et l'amélioration de la pertinence des réponses.

c. Compatibilité : Le format Markdown est largement pris en charge par les outils et les plateformes de traitement de texte, ce qui facilite son intégration dans les pipelines de traitement du langage naturel.

Azure Document Intelligence dispose d'un studio en ligne qui permet de visualiser et de convertir le contenu des documents PDF en Markdown. C'est très pratique pour visualiser le résultat de l'OCR et vérifier la qualité de la conversion.
![Azure Document Intelligence Studio](images/02_from_pdf_to_markdown.png)

3. Intégration du Markdown dans le RAG
--------------------------------------

Pour intégrer le Markdown dans le RAG, nous avons adapté le pipeline d'indexation des documents. Dans un premier temps, on effectue l'OCR des documents PDF avec Azure Document Intelligence pour obtenir le contenu en Markdown. Ensuite, on indexe ce contenu dans la base vectorielle pour permettre au modèle RAG de le récupérer et de générer des réponses.

Le code complet de l'intégration du Markdown dans le RAG est disponible dans le fichier [1-getting-started-with-ocr.ipynb](../notebooks/1-getting-started-with-ocr.ipynb).

Voici la partie du code C# qui illustre cette intégration :
- Initialisation du client Azure Document Intelligence
```csharp
var docIntelEndpoint =  env["AZURE_DOCUMENT_INTELLIGENCE_ENDPOINT"];
var credential = new DefaultAzureCredential();
var docIntelClient = new DocumentIntelligenceClient(new Uri(docIntelEndpoint), credential);
```
- Conversion du contenu des documents PDF en Markdown
```csharp
string folderPath = "../data";
string[] pdfFiles = Directory.GetFiles(folderPath, "*.pdf", SearchOption.AllDirectories);

foreach (var pdfFile in pdfFiles)
{
    var markdownFilePath = $"{pdfFile}.md";
    if (File.Exists(markdownFilePath))
    {
        Console.WriteLine($"Skipping {pdfFile} because it already has a markdown file");
        return;
    }

    try
    {
        using var fileStream = File.OpenRead(pdfFile);
        var binaryData = BinaryData.FromStream(fileStream);
        var analyzeRequest = new AnalyzeDocumentContent
        {
            Base64Source = binaryData
        };
        var result = await docIntelClient.AnalyzeDocumentAsync(waitUntil: WaitUntil.Completed, "prebuilt-layout", analyzeRequest: analyzeRequest, outputContentFormat: ContentFormat.Markdown);
        var markdownContent = result.Value.Content;
        await File.WriteAllTextAsync(markdownFilePath, markdownContent);
        Console.WriteLine($"Created: {markdownFilePath}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erreur lors du traitement OCR de {pdfFile}: {ex.Message}");
    }
}
```
- Indexation du contenu Markdown dans la base vectorielle
```csharp
string[] markdownFiles = Directory.GetFiles(folderPath, "*.md", SearchOption.AllDirectories);
foreach (string filePath in markdownFiles)
{
    string fileName = Path.GetFileName(filePath);
    string fullPath = Path.GetFullPath(filePath);
    
    await memory.ImportDocumentAsync(fullPath, documentId: fileName);

    Console.WriteLine("Successfully imported File Name: " + fileName);
}
```

4. Résultats et conclusions
---------------------------

Après avoir intégré Azure Document Intelligence et le format Markdown dans le pipeline du RAG, nous avons constaté une nette amélioration de la performance du modèle. Les tableaux sont correctement pris en compte et le modèle est capable de répondre aux questions qui s'y réfèrent.

![Tableau pris en compte](images/03_answered_question_cost_per_employee.png)

En résumé, l'intégration d'Azure Document Intelligence et l'utilisation du format Markdown ont permis d'améliorer la performance du RAG en prenant en compte les tableaux et en fournissant une sémantique supplémentaire au modèle de langage. Cette approche a démontré son efficacité et nous allons l'appliquer dans la suite de ce projet.