using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DotNetEnv;

namespace SalesTrack.KPIService
{
    public class OpenAiKpiService
    {
        public OpenAiKpiService()
        {
            // load the .env file containing OPENAI_API_KEY
            Env.Load(); // looks for .env in the current working directory
        }

        public async Task<string> AnalyzeSalesAsync(string jsonData)
        {
            // get your secret OpenAI key from environment
            string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            var requestBody = new
            {
                model = "gpt-4-turbo",
                messages = new[]
                {
                    new { role = "system", content = "You are a KPI analyst." },
                    new { role = "user", content = $"Analyze the following sales data: {jsonData}" }
                }
            };

            string json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
            string responseJson = await response.Content.ReadAsStringAsync();

            // parse response and extract assistant's message
            using var doc = JsonDocument.Parse(responseJson);
            string message = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return message;
        }
    }
}
