using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using CRUD_API.Context;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

public class AiService
{
    private readonly ChatClient _chatClient;
    private readonly CrudContext _context;

    public AiService(IConfiguration config, CrudContext context)
    {
        _context = context;
        var endpoint = new Uri(config["AzureOpenAI:Endpoint"]);
        var apiKey = config["AzureOpenAI:Key"];
        var deployment = config["AzureOpenAI:DeploymentName"];
        var azureClient = new AzureOpenAIClient(endpoint, new AzureKeyCredential(apiKey));
        _chatClient = azureClient.GetChatClient(deployment);
    }

    public async Task<string> AskAI(string message)
    {
        var teachers = await _context.Teachers
            .Include(t => t.Department)
            .Select(t => new
            {
                t.Id,
                t.Name,
                t.Email,
                t.Remark,
                Department = t.Department != null ? t.Department.Name : null,
                t.DepartmentID
            })
            .ToListAsync();

        var departments = await _context.Departments
            .Select(d => new { d.Id, d.Name, d.Remark })
            .ToListAsync();

        var teachersJson = JsonSerializer.Serialize(teachers);
        var departmentsJson = JsonSerializer.Serialize(departments);

        // JSON format examples as plain string (no interpolation needed)
        var jsonFormats = @"
Add teacher:
{""action"":""add_teacher"",""name"":""..."",""email"":""..."",""remark"":""..."",""departmentId"":1}

Update teacher:
{""action"":""update_teacher"",""id"":1,""name"":""..."",""email"":""..."",""remark"":""..."",""departmentId"":1}

Delete teacher:
{""action"":""delete_teacher"",""id"":1}

Add department:
{""action"":""add_department"",""name"":""..."",""remark"":""...""}

Update department:
{""action"":""update_department"",""id"":1,""name"":""..."",""remark"":""...""}

Delete department:
{""action"":""delete_department"",""id"":1}";

        var systemPrompt =
            "You are an assistant for managing teachers and departments in a college system.\n\n" +
            "Current Teachers in DB:\n" + teachersJson + "\n\n" +
            "Current Departments in DB:\n" + departmentsJson + "\n\n" +
            "RULES:\n" +
            "- ADD teacher: check if department exists. If NOT, reply in plain text telling user to add department first. If YES, reply with JSON only.\n" +
            "- UPDATE teacher: check if teacher exists by name or id. If not found, say so in plain text.\n" +
            "- DELETE teacher: check if teacher exists. If not found, say so in plain text.\n" +
            "- ADD department: check if it already exists. If yes, say so in plain text.\n" +
            "- UPDATE/DELETE department: check if it exists first.\n" +
            "- For CRUD actions respond ONLY with a valid JSON object — no extra text, no markdown, no explanation.\n" +
            "- For normal conversation or when validation fails, respond with plain text.\n\n" +
            "JSON formats to use:\n" + jsonFormats;

        var completion = await _chatClient.CompleteChatAsync(
        [
            new SystemChatMessage(systemPrompt),
            new UserChatMessage(message)
        ]);

        return completion.Value.Content[0].Text;
    }
}