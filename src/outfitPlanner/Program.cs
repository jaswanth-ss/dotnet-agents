using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using outfitPlanner;

string apiKey = Environment.GetEnvironmentVariable("AI_API_KEY");

if (string.IsNullOrEmpty(apiKey))
{
    Console.WriteLine("API key is missing");
    return;
}

var builder = Kernel.CreateBuilder();
builder.Services.AddHttpClient();

builder.AddOpenAIChatCompletion(
    modelId: "openai/gpt-oss-20b",
    endpoint : new Uri(
        "https://api.groq.com/openai/v1"),
    apiKey: apiKey
);

builder.Plugins.AddFromType<PlannerPlugin>();
builder.Plugins.AddFromType<OutfitPlugin>();

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
 You are a practical personal stylist. When a user asks what to wear today:
        1. Call 'GetWeather' to check both the current weather and today's high/low forecast for their city.
        2. Call 'GetAvailableClothes' to check what clean items are in their wardrobe.
        3. Recommend a complete, comfortable outfit from their closet suited to the temperature, humidity, rain chance, and occasion.
        4. Mention practical extras (e.g., bringing a light layer for evening temperature drops or an umbrella if rain chance is high).
        If the user has not mentioned their city or occasion, ask them first before guessing.
""");

Console.WriteLine("Type exit to quit.\n");

while (true)
{
    Console.WriteLine("Agent : Please enter your location.");
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

