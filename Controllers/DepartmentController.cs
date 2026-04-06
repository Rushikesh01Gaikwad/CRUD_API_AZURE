using CRUD_API.Context;
using CRUD_API.Models;
using CRUD_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRUD_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private ReturnData rtn = new ReturnData();
        private readonly CrudContext _Context;

        public DepartmentController(CrudContext context)
        {
            _Context = context;
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var data = await _Context.Departments.ToListAsync();
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
        public async Task<IActionResult> Insert(DepartmentClass department)
        {
            try
            {
                var existing = await _Context.Departments.AnyAsync(d => d.Name.ToLower() == department.Name.ToLower());
                if(existing)
                {
                    rtn.Message = "Department with the same name already exists";
                    rtn.Status = 0;
                    return Ok(rtn);
                }
                _Context.Departments.Add(department);
                await _Context.SaveChangesAsync();
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
        public async Task<IActionResult> Update(int id, [FromBody] DepartmentClass department)
        {
            try
            {
                var existingDepartment = await _Context.Departments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (existingDepartment == null)
                {
                    rtn.Message = "Department not found";
                    rtn.Status = 0;
                    return Ok(rtn);
                }
                else
                {
                    _Context.Departments.Update(department);
                    await _Context.SaveChangesAsync();
                    rtn.Message = "Data Updated successfully";
                }

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
                var existingDepartment = await _Context.Departments.FindAsync(id);
                if (existingDepartment == null)
                {
                    rtn.Message = "Department not found";
                    rtn.Status = 0;
                    return Ok(rtn);
                }
                else
                {
                    _Context.Departments.Remove(existingDepartment);
                    await _Context.SaveChangesAsync();
                    rtn.Message = "Department Deleted successfully";
                }
            }
            catch (Exception ex)
            {
                rtn.Message = "Error deleting Department";
                rtn.Status = 0;
                rtn.Exception = ex.Message;
            }
            return Ok(rtn);
        }
    }
}
