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
        private readonly BlobService _blobService;


        public UploadController(CrudContext context, BlobService blobService)
        {
            _Context = context;
            _blobService = blobService;
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

                var fileUrl = await _blobService.UploadFileAsync(file);

                rtn.Data = new
                {
                    filePath = fileUrl // 🔥 Azure URL
                };

                return Ok(rtn);
            }
            catch (Exception ex)
            {
                rtn.Status = 0;
                rtn.Message = ex.Message;
                return Ok(rtn);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTeacherPhoto(IFormFile file, string oldFileUrl)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    rtn.Status = 0;
                    rtn.Message = "File not found!";
                    return Ok(rtn);
                }

                // 🔥 Delete old file from blob
                if (!string.IsNullOrEmpty(oldFileUrl))
                {
                    await _blobService.DeleteFileAsync(oldFileUrl);
                }

                var fileUrl = await _blobService.UploadFileAsync(file);

                rtn.Status = 1;
                rtn.Data = new
                {
                    filePath = fileUrl
                };

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
