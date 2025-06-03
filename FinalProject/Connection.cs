using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Markdig;
using DotNetEnv;

namespace wpf_resipe
{
    public class Connection
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public static async Task<string> GetResponseFromAI(string userMessage)
        {
            try
            {
                DotNetEnv.Env.Load();
                Console.WriteLine("User Creating Request to AI from server");

                string apiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY");
                string model = Environment.GetEnvironmentVariable("OPENROUTER_MODEL_NAME");

                var request = new
                {
                    model = model,
                    messages = new[]
                    {
                    new { role = "system", content = "You are an AI assistant." },
                    new { role = "user", content = userMessage }
                },
                    temperature = 0.7,
                    max_tokens = 1500
                };

                var requestJson = JsonSerializer.Serialize(request);
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://openrouter.ai/api/v1/chat/completions")
                {
                    Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
                };

                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                httpRequest.Headers.Add("HTTP-Referer", "catshredia-app.com");
                httpRequest.Headers.Add("X-Title", "mathai");

                var response = await httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Ошибка HTTP: " + response.StatusCode);
                    Console.WriteLine("Ответ: " + responseContent);
                    throw new Exception("Ошибка ответа API");
                }

                using (var document = JsonDocument.Parse(responseContent))
                {
                    string aiMessage = document.RootElement
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString();

                    // Markdown → HTML
                    var html = Markdown.ToHtml(aiMessage);
                    if (html.Contains("<head>"))
                    {
                        html = html.Replace("<head>", "<head><meta charset=\"UTF-8\">");
                    }
                    else
                    {
                        html = "<html><head><meta charset=\"UTF-8\"></head><body>" + html + "</body></html>";
                    }
                    return html;
                }

                
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("\tError: " + ex.Message);
                throw;
            }
        }
    }
}
