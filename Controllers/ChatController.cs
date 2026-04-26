using CRUD_API.Context;
using CRUD_API.Models;
using CRUD_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CRUD_API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ChatController : ControllerBase
    {
        private ReturnData rtn = new ReturnData();
        private readonly AiService _ai;
        private readonly CrudContext _context;

        public ChatController(AiService ai, CrudContext context)
        {
            _ai = ai;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Ask([FromBody] ChatRequest request)
        {
            try
            {
                var response = await _ai.AskAI(request.Message);

                var trimmed = response.Trim();
                if (trimmed.StartsWith("{"))
                {
                    var action = JsonSerializer.Deserialize<AiAction>(trimmed, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (action != null)
                    {
                        var result = await HandleAction(action);
                        rtn.Data = result;
                        rtn.Message = result;
                        return Ok(rtn);
                    }
                }

                // Plain text response
                rtn.Data = response;
                rtn.Message = response;
                return Ok(rtn);
            }
            catch (Exception ex)
            {
                rtn.Status = 0;
                rtn.Message = "Error processing request";
                rtn.Exception = ex.Message;
                return Ok(rtn);
            }
        }

        private async Task<string> HandleAction(AiAction action)
        {
            switch (action.Action?.ToLower())
            {
                // ─── TEACHER ACTIONS ───────────────────────────────

                case "add_teacher":
                    {
                        var teacher = new TeacherClass
                        {
                            Name = action.Name!,
                            Email = action.Email ?? "",
                            Remark = action.Remark,
                            DepartmentID = action.DepartmentId
                        };
                        _context.Teachers.Add(teacher);
                        await _context.SaveChangesAsync();
                        return $"Teacher '{teacher.Name}' added successfully.";
                    }

                case "update_teacher":
                    {
                        var teacher = await _context.Teachers.FindAsync(action.Id);
                        if (teacher == null)
                            return $"Teacher with ID {action.Id} not found.";

                        teacher.Name = action.Name ?? teacher.Name;
                        teacher.Email = action.Email ?? teacher.Email;
                        teacher.Remark = action.Remark ?? teacher.Remark;
                        teacher.DepartmentID = action.DepartmentId ?? teacher.DepartmentID;

                        await _context.SaveChangesAsync();
                        return $"Teacher '{teacher.Name}' updated successfully.";
                    }

                case "delete_teacher":
                    {
                        var teacher = await _context.Teachers.FindAsync(action.Id);
                        if (teacher == null)
                            return $"Teacher with ID {action.Id} not found.";

                        _context.Teachers.Remove(teacher);
                        await _context.SaveChangesAsync();
                        return $"Teacher '{teacher.Name}' deleted successfully.";
                    }

                // ─── DEPARTMENT ACTIONS ────────────────────────────

                case "add_department":
                    {
                        var dept = new DepartmentClass
                        {
                            Name = action.Name!,
                            Remark = action.Remark
                        };
                        _context.Departments.Add(dept);
                        await _context.SaveChangesAsync();
                        return $"Department '{dept.Name}' added successfully.";
                    }

                case "update_department":
                    {
                        var dept = await _context.Departments.FindAsync(action.Id);
                        if (dept == null)
                            return $"Department with ID {action.Id} not found.";

                        dept.Name = action.Name ?? dept.Name;
                        dept.Remark = action.Remark ?? dept.Remark;

                        await _context.SaveChangesAsync();
                        return $"Department '{dept.Name}' updated successfully.";
                    }

                case "delete_department":
                    {
                        var dept = await _context.Departments
                            .Include(d => d.Teachers)
                            .FirstOrDefaultAsync(d => d.Id == action.Id);

                        if (dept == null)
                            return $"Department with ID {action.Id} not found.";

                        if (dept.Teachers != null && dept.Teachers.Any())
                            return $"Cannot delete '{dept.Name}' — it has {dept.Teachers.Count} teacher(s) assigned. Reassign them first.";

                        _context.Departments.Remove(dept);
                        await _context.SaveChangesAsync();
                        return $"Department '{dept.Name}' deleted successfully.";
                    }

                default:
                    return "Unknown action received from AI.";
            }
        }
    }

    // ─── MODELS ───────────────────────────────────────────────────

    public class ChatRequest
    {
        public string Message { get; set; }
    }

    public class AiAction
    {
        public string? Action { get; set; }
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Remark { get; set; }
        public int? DepartmentId { get; set; }
    }
}