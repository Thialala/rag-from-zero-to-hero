public class RAGWorkflow
{
    private readonly DocumentIntelligenceClient _docIntelClient;
    private readonly MemoryServerless _memory;

    public RAGWorkflow(DocumentIntelligenceClient docIntelClient, MemoryServerless memory)
    {
        _docIntelClient = docIntelClient;
        _memory = memory;
    }

    public async Task RunAsync(string folderPath)
    {
        await OCROnPDFFilesAsync(folderPath);
        await ImportMarkdownFilesAsync(folderPath);
    }

    private async Task OCROnPDFFilesAsync(string folderPath)
    {
        string[] pdfFiles = Directory.GetFiles(folderPath, "*.pdf", SearchOption.AllDirectories);

        foreach (var pdfFile in pdfFiles)
        {
            var markdownFilePath = $"{pdfFile}.md";
            if (File.Exists(markdownFilePath))
            {
                Console.WriteLine($"Skipping {pdfFile} because it already has a markdown file");
                continue;
            }

            try
            {
                using var fileStream = File.OpenRead(pdfFile);
                var binaryData = BinaryData.FromStream(fileStream);
                var analyzeRequest = new AnalyzeDocumentContent
                {
                    Base64Source = binaryData
                };
                var result = await _docIntelClient.AnalyzeDocumentAsync(
                    waitUntil: WaitUntil.Completed, 
                    "prebuilt-layout", 
                    analyzeRequest: analyzeRequest, 
                    outputContentFormat: ContentFormat.Markdown
                );
                var markdownContent = result.Value.Content;
                await File.WriteAllTextAsync(markdownFilePath, markdownContent);
                Console.WriteLine($"Created: {markdownFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing OCR for {pdfFile}: {ex.Message}");
            }
        }
    }
    
    private async Task ImportMarkdownFilesAsync(string folderPath)
    {
        string[] markdownFiles = Directory.GetFiles(folderPath, "*.md", SearchOption.AllDirectories);
        foreach (string filePath in markdownFiles)
        {
            string fileName = Path.GetFileName(filePath);
            string fullPath = Path.GetFullPath(filePath);

            await _memory.ImportDocumentAsync(fullPath, documentId: fileName);

            Console.WriteLine("Successfully imported File Name: " + fileName);
        }
    }

    public async Task AskQuestionAsync(string question)
    {
        var answer = await _memory.AskAsync(question);
        Console.WriteLine($"Question: {question}\n\nAnswer: {answer.Result}");
    }
}