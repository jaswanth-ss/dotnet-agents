using fileOrganizer.Agents;
using fileOrganizer.Tools;
using Microsoft.Agents.AI;

string? apiKey = Environment.GetEnvironmentVariable("AI_API_KEY");

if (string.IsNullOrEmpty(apiKey))
{
    Console.WriteLine("API Key is missing");
    return;
}

string testFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "TestFolder");

var tools = new OrganizerTool(testFolderPath);
AIAgent agent = OrganizerAgent.Create(apiKey, tools);

AgentSession agentSession = await agent.CreateSessionAsync();

Console.WriteLine($"Sandboxed to : {tools.sandBoxPath}");

while (true)
{
    Console.Write("You: ");
    string? input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input) || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
        break;
    AgentResponse response = await agent.RunAsync(input, agentSession);
    Console.WriteLine($"\nAgent: {response.Text}\n");
}