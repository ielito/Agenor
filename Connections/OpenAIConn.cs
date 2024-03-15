﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class ChatGptClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl;

    public ChatGptClient(string? apiKey)
    {
        if (string.IsNullOrEmpty(apiKey)) throw new ArgumentException("API key is required.");

        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        _apiUrl = "https://api.openai.com/v1/chat/completions";
    }

    public async Task<string> GetResponseAsync(string message, int maxAttempts = 3)
    {
        int attempt = 0;
        while (attempt < maxAttempts)
        {
            var requestData = new
            {
                model = "gpt-3.5-turbo-0613", // Especifica o modelo
                max_tokens = 300, // Ajuste conforme necessário
                temperature = 0.7,
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = "Você é Agenor, um assistente virtual amigável e animado. Você gosta de usar emojis e oferecer sugestões úteis. Sempre que um endereço for mencionado, tente sugerir um café nas proximidades."
                    },

                    new { role = "user", content = message },
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

            try
            {
                using (var response = await _httpClient.PostAsync(_apiUrl, content))
                {
                    response.EnsureSuccessStatusCode();
                    var responseContent = await response.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject(responseContent);
                    return result.choices[0].message.content.ToString();
                }
            }
            catch (HttpRequestException httpEx) when (httpEx.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                attempt++;
                Console.WriteLine("Atingido o limite de taxa, tentando novamente após delay...");
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt))); // Delay exponencial
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao chamar a API do ChatGPT: {ex.Message}");
                return null;
            }
        }

        throw new Exception("Número máximo de tentativas atingido");
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}