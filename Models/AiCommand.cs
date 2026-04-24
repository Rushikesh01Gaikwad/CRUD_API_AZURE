using System.Text.Json;

public class AiCommand
{
    public string Action { get; set; } = "";
    public JsonElement Data { get; set; }
}