using CRUD_API.Context;
using CRUD_API.Models;
using CRUD_API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CRUD_API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ChatController : ControllerBase
    {
        private readonly AiService _aiService;
        private readonly CrudContext _context;

        public ChatController(AiService aiService, CrudContext context)
        {
            _aiService = aiService;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Ask([FromBody] dynamic body)
        {
            string question = body.question;

            var aiResponse = await _aiService.Ask(question);

            var parsed = JsonDocument.Parse(aiResponse);
            var action = parsed.RootElement.GetProperty("action").GetString();

            switch (action)
            {
                case "add_teacher":
                    var t = parsed.RootElement.GetProperty("data");

                    var teacher = new TeacherClass
                    {
                        Name = t.GetProperty("name").GetString(),
                        Email = t.GetProperty("email").GetString(),
                        Id = t.GetProperty("departmentId").GetInt32()
                    };

                    _context.Teachers.Add(teacher);
                    await _context.SaveChangesAsync();

                    return Ok(new { answer = "Teacher added successfully" });

                case "delete_teacher":
                    int id = parsed.RootElement.GetProperty("data").GetProperty("id").GetInt32();

                    var existing = await _context.Teachers.FindAsync(id);
                    if (existing == null)
                        return Ok(new { answer = "Teacher not found" });

                    _context.Teachers.Remove(existing);
                    await _context.SaveChangesAsync();

                    return Ok(new { answer = "Teacher deleted" });

                case "add_department":
                    var d = parsed.RootElement.GetProperty("data");

                    var dept = new DepartmentClass
                    {
                        Name = d.GetProperty("name").GetString()
                    };

                    _context.Departments.Add(dept);
                    await _context.SaveChangesAsync();

                    return Ok(new { answer = "Department added" });

                default:
                    return Ok(new { answer = "Sorry, I didn't understand." });
            }
        }

    }
}
