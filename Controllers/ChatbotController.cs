using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace StudentInformationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ChatbotController> _logger;
        private readonly string _openAiApiKey;
        private const string OpenAiUrl = "https://api.openai.com/v1/chat/completions";

        public ChatbotController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<ChatbotController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;

            // Try to get API key from configuration/environment variables
            _openAiApiKey = _configuration["OPENAI_API_KEY"] ??
                           Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            if (string.IsNullOrEmpty(_openAiApiKey))
            {
                _logger.LogError("OpenAI API key not found in configuration or environment variables");
                throw new InvalidOperationException("OpenAI API key not found in configuration or environment variables");
            }
            else
            {
                _logger.LogInformation("OpenAI API key successfully loaded from configuration");
            }
        }

        public class ChatRequest
        {
            public string? Message { get; set; }
        }

        public class ChatResponse
        {
            public string Response { get; set; } = string.Empty;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ChatRequest request)
        {
            if (string.IsNullOrEmpty(request.Message))
            {
                return BadRequest("Message cannot be empty");
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openAiApiKey}");

                // Prepare system message with context about the Student Information System
                var systemMessage = @"You are a knowledgeable assistant for the Student Information System (SIS), a comprehensive web application for managing educational data.
                
                You can help with questions about students, courses, enrollments, and system features.
                
                Keep responses helpful, concise, and friendly.";

                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "system", content = systemMessage },
                        new { role = "user", content = request.Message }
                    },
                    max_tokens = 500,
                    temperature = 0.7
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json"
                );

                _logger.LogInformation("Sending request to OpenAI API");
                var response = await client.PostAsync(OpenAiUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error from OpenAI API: {StatusCode}, {Content}",
                        response.StatusCode, errorContent);
                    return StatusCode((int)response.StatusCode,
                        $"Error from OpenAI API: {response.StatusCode}");
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var responseData = JsonSerializer.Deserialize<JsonElement>(responseString);

                if (responseData.TryGetProperty("choices", out var choices) &&
                    choices.GetArrayLength() > 0 &&
                    choices[0].TryGetProperty("message", out var message) &&
                    message.TryGetProperty("content", out var content1))
                {
                    var botResponse = content1.GetString() ?? "Sorry, I couldn't generate a response.";
                    return Ok(new ChatResponse { Response = botResponse });
                }

                _logger.LogError("Unexpected response format from OpenAI API: {Response}", responseString);
                return StatusCode(500, "Unexpected response format from OpenAI API");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request to OpenAI API");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}