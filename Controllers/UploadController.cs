using CRUD_API.Context;
using CRUD_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CRUD_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private ReturnData rtn = new ReturnData();
        private readonly CrudContext _Context;

        public UploadController(CrudContext context)
        {
            _Context = context;
        }

        [HttpPost]
        public async Task<IActionResult> UploadTeacherPhoto(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    rtn.Status = 0;
                    rtn.Message = "File not found!";
                    return Ok(rtn);
                }

                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/teachers");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                rtn.Data = new { FileName = fileName, filePath = "/uploads/teachers/" + fileName };

                return Ok(rtn);
            }
            catch (Exception ex)
            {
                rtn.Status = 0;
                rtn.Message = ex.Message;
                return Ok(rtn);
            }
        }

    }
}
