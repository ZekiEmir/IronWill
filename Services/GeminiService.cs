using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IronWill.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["GeminiApiKey"];
        }

        public async Task<string> GenerateResponseAsync(string systemPrompt, string userMessage)
        {
            if (string.IsNullOrEmpty(_apiKey)) return "HATA: API Anahtarı bulunamadı.";

            // 1. Try a default stable model first
            string defaultModel = "gemini-1.5-flash";
            var response = await CallGeminiAsync(defaultModel, systemPrompt, userMessage);
            if (response != null) return response;

            // 2. If default fails, Auto-Discover available models
            var availableModel = await FindFirstAvailableModelAsync();
            if (availableModel != null)
            {
                var retryResponse = await CallGeminiAsync(availableModel, systemPrompt, userMessage);
                if (retryResponse != null) return retryResponse;
            }

            return "BAĞLANTI HATASI: API Anahtarınızla uyumlu hiçbir model bulunamadı veya 'Generative Language API' Google Cloud Console'da etkinleştirilmemiş.";
        }

        private async Task<string?> CallGeminiAsync(string model, string systemPrompt, string userMessage)
        {
            var content = CreateRequest(systemPrompt, userMessage);
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={_apiKey}";
            
            var response = await _httpClient.PostAsync(url, content);
            if (!response.IsSuccessStatusCode) return null;

            try 
            {
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                return doc.RootElement.GetProperty("candidates")[0]
                                      .GetProperty("content")
                                      .GetProperty("parts")[0]
                                      .GetProperty("text")
                                      .GetString();
            }
            catch { return null; }
        }

        private StringContent CreateRequest(string system, string user)
        {
            var requestBody = new
            {
                contents = new[] { new { parts = new[] { new { text = system + "\n\nKULLANICI: " + user } } } }
            };
            return new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        }

        private async Task<string?> FindFirstAvailableModelAsync()
        {
            // List models
            string url = $"https://generativelanguage.googleapis.com/v1beta/models?key={_apiKey}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            
            // Find first model that supports generateContent
            foreach (var model in doc.RootElement.GetProperty("models").EnumerateArray())
            {
                var name = model.GetProperty("name").GetString(); // e.g. "models/gemini-1.5-pro"
                var methods = model.GetProperty("supportedGenerationMethods");
                
                foreach (var method in methods.EnumerateArray())
                {
                    if (method.GetString() == "generateContent")
                    {
                        // Remove "models/" prefix if present for the URL construction
                        return name.Replace("models/", ""); 
                    }
                }
            }
            return null;
        }
    }
}
