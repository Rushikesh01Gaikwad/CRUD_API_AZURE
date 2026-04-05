using CRUD_API.Context;
using CRUD_API.Models;
using CRUD_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRUD_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private ReturnData rtn = new ReturnData();
        private readonly CrudContext _Context;

        public LoginController(CrudContext context)
        {
            _Context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                if (username == "admin" && password == "password")
                {
                    rtn.Message = "Login successful";
                    rtn.Data = new { Token = "fake-jwt-token" };
                }
                else
                {
                    rtn.Message = "Invalid username or password";
                    rtn.Status = 0;
                }
            }
            catch (Exception ex)
            {
                rtn.Message = "Error during login";
                rtn.Status = 0;
                rtn.Exception = ex.Message;
            }
            return Ok(rtn);
        }

        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] UserClass user)
        {
            try
            {
                if(user != null)
                {
                    var existing = await _Context.Users.AnyAsync(u => u.Name.ToLower() == user.Name.ToLower());
                    if (existing)
                    {
                        rtn.Message = "User with the same name already exists";
                        rtn.Status = 0;
                        return Ok(rtn);
                    }
                    else
                    {
                        _Context.Users.Add(user);
                        await _Context.SaveChangesAsync();
                        rtn.Message = "User registered successfully";
                        return Ok(rtn);
                    }
                }
            }
            catch (Exception ex)
            {
                rtn.Message = "Error during logout";
                rtn.Status = 0;
                rtn.Exception = ex.Message;
            }
            return Ok(rtn);
        }
    }
}
