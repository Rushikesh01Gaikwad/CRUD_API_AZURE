using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace CRUD_API.Services
{
    public class AiService
    {
        private readonly ChatClient _chatClient;

        public AiService(IConfiguration config)
        {
            _chatClient = new ChatClient(
                model: config["AzureOpenAI:Deployment"],
                credential: new ApiKeyCredential(config["AzureOpenAI:Key"]),
                options: new OpenAIClientOptions
                {
                    Endpoint = new Uri(config["AzureOpenAI:Endpoint"])
                }
            );
        }

        public async Task<string> Ask(string userInput)
        {
            var messages = new List<ChatMessage>
        {
            ChatMessage.CreateSystemMessage(@"
You are a college management assistant.

Convert user input into STRICT JSON.

Rules:
- Use exact property names
- Teacher: Name, Email, Remark, DepartmentID
- If department name is given → use DepartmentName

Actions:
add_teacher
delete_teacher
add_department
delete_department

Examples:

User: Add teacher John email john@test.com department 1
Output:
{
  ""action"": ""add_teacher"",
  ""data"": {
    ""Name"": ""John"",
    ""Email"": ""john@test.com"",
    ""DepartmentID"": 1
  }
}

User: Add teacher Ravi in Computer department
Output:
{
  ""action"": ""add_teacher"",
  ""data"": {
    ""Name"": ""Ravi"",
    ""DepartmentName"": ""Computer""
  }
}

User: Delete teacher 5
Output:
{
  ""action"": ""delete_teacher"",
  ""data"": { ""Id"": 5 }
}
"),
            ChatMessage.CreateUserMessage(userInput)
        };

            var response = await _chatClient.CompleteChatAsync(messages);

            return response.Value.Content[0].Text;
        }
    }
}
