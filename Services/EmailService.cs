using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CRUD_API.Services
{
    public class EmailService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(HttpClient httpClient, IConfiguration configuration, ILogger<EmailService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string message)
        {
            try
            {
                // 🔐 Get Azure Function URL
                var functionUrl = _configuration["AzureFunctionUrl"];

                if (string.IsNullOrWhiteSpace(functionUrl))
                {
                    _logger.LogError("AzureFunctionUrl is missing in configuration.");
                    return false;
                }

                // 📦 Prepare request body
                var data = new
                {
                    to = to,
                    subject = subject,
                    message = message
                };

                var jsonContent = new StringContent(
                    JsonConvert.SerializeObject(data),
                    Encoding.UTF8,
                    "application/json"
                );

                // 📤 Call Azure Function
                var response = await _httpClient.PostAsync(functionUrl, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Email sent successfully to {Email}", to);
                    return true;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Email failed. Status: {Status}, Error: {Error}", response.StatusCode, error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while sending email");
                return false;
            }
        }
    }
}