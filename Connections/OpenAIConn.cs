using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;

public class ChatGptClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl = "https://api.openai.com.com/v1/chat/completions";

    // Construtor público e sem parametros - prereq da OutSystems
    public ChatGptClient()
    {
        _httpClient = new HttpClient();
    }

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
                        content = "Como Agenor, meu assistente virtual, você tem acesso a uma base de dados detalhada de pedidos. Para cada interação, você deve fornecer informações precisas e úteis sobre o status dos pedidos, datas estimadas de entrega, e informações relevantes sobre a localização do pedido. Use um tom amigável e engajador, e quando apropriado, sugira locais como cafés próximos à localização do pedido. Lembre-se de adaptar suas respostas com base nas especificidades de cada pedido e nas perguntas do usuário."
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

    public string GetResponseSync(string message, int maxAttempts = 3)
    {
        try
        {
            var task = GetResponseAsync(message, maxAttempts);
            task.Wait();
            return task.Result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Falha ao obter resposta sincronamente.");
            return "Erro ao obter resposta.";
        }
    }

}