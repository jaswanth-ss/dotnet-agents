using agenticCalculator;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

string apiKey = Environment.GetEnvironmentVariable("AI_API_KEY");
if (string.IsNullOrEmpty(apiKey))
{
    Console.WriteLine("API Key Is Required");
    return;
}

using var httpClient = new HttpClient();

var builder = Kernel.CreateBuilder();

builder.AddOpenAIChatCompletion(
    modelId : "openai/gpt-oss-20b",
    endpoint : new Uri(
        "https://api.groq.com/openai/v1"),
    apiKey: apiKey,
    httpClient: httpClient
);

builder.Plugins.AddFromType<CalculatorPlugin>("Calculator");

Kernel kernel = builder.Build();

var chatService =
    kernel.GetRequiredService<IChatCompletionService>();

var executionSettings =
    new OpenAIPromptExecutionSettings
    {
        FunctionChoiceBehavior =
            FunctionChoiceBehavior.Auto()
    };

var history = new ChatHistory();

history.AddSystemMessage("""
You are an intelligent Agentic Calculator.

Your responsibilities:

1. Understand natural language mathematical questions.
2. Use Calculator plugin functions to perform calculations.
3. Never invent calculation results.
4. For multiple calculations, invoke tools in
   the correct sequence.
5. Use the previous tool result for the next operation.
6. Explain every calculation step.
7. Do not answer unrelated questions.
8. Ask clarification if the request is ambiguous.
""");

Console.WriteLine("Type exit to quit.\n");

while (true)
{
    Console.WriteLine("You: ");
    string userInput = Console.ReadLine();

    if (string.Equals(userInput, "exit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Exiting....");
        break;
    }

    history.AddUserMessage(userInput);

    try
    {
        Console.WriteLine("Agent Processing your request...");

        var response = await chatService.GetChatMessageContentAsync(
            history,
            executionSettings: executionSettings,
            kernel: kernel
        );

        string answer = response.Content ?? "";
        history.AddAssistantMessage(answer);

        Console.WriteLine("\nAgent:");

        Console.WriteLine(answer);
    }
    catch (Exception ex)
    {
        Console.WriteLine("\nAgent Error:");
        Console.WriteLine(ex.Message);
    }

    Console.WriteLine(
        "\n--------------------------------------\n");
}