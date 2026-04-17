using Azure.Storage.Blobs;

namespace CRUD_API.Services
{
    public class BlobService
    {
        private readonly string _connectionString;
        private readonly string _containerName = "teachers";

        public BlobService(IConfiguration configuration)
        {
            _connectionString = configuration["AzureBlobStorage"];
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            await containerClient.CreateIfNotExistsAsync();

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            var blobClient = containerClient.GetBlobClient(fileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            return blobClient.Uri.ToString(); // 🔥 return URL
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            var fileName = Path.GetFileName(fileUrl);
            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.DeleteIfExistsAsync();
        }
    }
}