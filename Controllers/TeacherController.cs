using CRUD_API.Context;
using CRUD_API.Models;
using CRUD_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRUD_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private ReturnData rtn = new ReturnData();
        private readonly CrudContext _Context;
        private readonly EmailService _emailService;

        public TeacherController(CrudContext context, EmailService emailService)
        {
            _Context = context;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var data = await _Context.Teachers.Include(s =>s.Department).ToListAsync();
                rtn.Data = data;
                rtn.Message = "Data Fetched successfully";
            }
            catch (Exception ex)
            {
                rtn.Message = "Error retrieving data";
                rtn.Status = 0;
                rtn.Exception = ex.Message;
            }
            return Ok(rtn);
        }

        [HttpPost]
        public async Task<IActionResult> Insert(TeacherClass teacher)
        {
            try
            {
                var existing = await _Context.Teachers.AnyAsync(t => t.Name.ToLower() == teacher.Name.ToLower());
                if (existing)
                {
                    rtn.Message = "Teacher with the same name already exists";
                    rtn.Status = 0;
                    return Ok(rtn);
                }
                _Context.Teachers.Add(teacher);
                rtn.Data = teacher;
                await _Context.SaveChangesAsync();
                //await _emailService.SendEmailAsync(
                //    teacher.Email, 
                //    "Welcome to Our System",
                //    $"Hello {teacher.Name},\n\nWelcome! Your account has been created successfully."
                //); //slower process

                //_ = Task.Run(async () =>
                //{
                //    await _emailService.SendEmailAsync(
                //        teacher.Email,
                //        "Welcome to Our System",
                //        $"Hello {teacher.Name}, Welcome!"
                //    );
                //}); //fast process
                rtn.Message = "Data Added successfully";
            }
            catch (Exception ex)
            {
                rtn.Message = "Error adding data";
                rtn.Status = 0;
                rtn.Exception = ex.Message;
            }
            return Ok(rtn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TeacherClass teacher)
        {
            try
            {
                var existingTeacher = await _Context.Teachers.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
                if (existingTeacher == null)
                {
                    rtn.Message = "Data not found";
                    rtn.Status = 0;
                    return Ok(rtn);
                }
                //_Context.Entry(existingTeacher).CurrentValues.SetValues(teacher);
                teacher.Department = null;
                _Context.Teachers.Update(teacher);
                await _Context.SaveChangesAsync();
                rtn.Message = "Data Updated successfully";
            }
            catch (Exception ex)
            {
                rtn.Message = "Error updating data";
                rtn.Status = 0;
                rtn.Exception = ex.Message;
            }
            return Ok(rtn);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var existingTeacher = await _Context.Teachers.FindAsync(id);
                if (existingTeacher == null)
                {
                    rtn.Message = "Data not found";
                    rtn.Status = 0;
                    return Ok(rtn);
                }
                _Context.Teachers.Remove(existingTeacher);
                await _Context.SaveChangesAsync();
                rtn.Message = "Data Deleted successfully";
            }
            catch (Exception ex)
            {
                rtn.Message = "Error deleting data";
                rtn.Status = 0;
                rtn.Exception = ex.Message;
            }
            return Ok(rtn);
        }
    }
}
