using System.ClientModel;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using fileOrganizer.Tools;

namespace fileOrganizer.Agents;

public static class OrganizerAgent
{
    private const string SystemInstructions = """
        You are a file-organizing assistant working exclusively inside the project's `TestFolder`.
        You do NOT need to ask the user for a folder path—always use `ListFiles` and `OrganizeFiles`.

        Follow the exact grouping mode requested by the user:
        - **Group by Type** ("by type", "by extension", "all pdfs together"):
          Group every file by extension category (`PDFs`, `Excel`, `Documents`, `Images`, `Archives`, `Others`).
        - **Group by Similar Name** ("by name", "similar names", "matching prefixes"):
          Group sets of 2 or more files sharing a common base name or prefix (e.g., `sample_1.pdf`, `sample_2.xlsx` -> `Sample/`). Leave standalone unrelated files untouched!
        - **Both** (only if user asks for both):
          Group matching file names first, then group remaining standalone files by file type.

        If the user's request is unclear, call `ListFiles` first, show them what files are inside `TestFolder`, and ask whether they want to group **by type**, **by similar name**, or **both**.
        """;

    public static AIAgent Create(string groqApiKey, OrganizerTool tools, string model = "openai/gpt-oss-120b")
    {
        var groqOptions = new OpenAIClientOptions
        {
            Endpoint = new Uri("https://api.groq.com/openai/v1")
        };

        IChatClient chatClient = new OpenAIClient(new ApiKeyCredential(groqApiKey), groqOptions)
            .GetChatClient(model)
            .AsIChatClient();

        return new ChatClientAgent(
            chatClient,
            new ChatClientAgentOptions
            {
                Name = "SandboxFileOrganizer",
                ChatOptions = new ChatOptions
                {
                    Instructions = SystemInstructions,
                    Tools = tools.ToAITools()
                }
            }
        );
    }
}
