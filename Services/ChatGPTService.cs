using System.Text;
using System.Threading.Tasks;
using Serilog;
using static OrdersApiService;

public class ChatGptService
{
    private readonly ChatGptClient _chatGptClient;
    private readonly MongoDbService? _mongoDbService;
    private string openAiApiKey;

    public ChatGptService(string openAiApiKey)
    {
        _chatGptClient = new ChatGptClient(openAiApiKey);
        this.openAiApiKey = openAiApiKey;
    }

    public async Task<string> ProcessOrdersAndGetResponse()
    {
        Log.Information("Buscando detalhes dos pedidos com _mongoDBService");

        if (_mongoDbService != null)

        {
            var orders = _mongoDbService.GetOrdersFromMongoDb();
            Log.Information($"Encontrados {orders.Count()} pedidos");

            string prompt = CreatePromptFromOrders(orders);
            return await _chatGptClient.GetResponseAsync(prompt);
        }
        else
        {
            Log.Warning("_mongoDBService é nulo ao tentar buscar detalhes dos pedidos");
            return "Não foi possível buscar os detalhes do pedido porque o _mongoDBSercice é nulo.";
        }
    }

    public string CreatePromptFromOrders(IEnumerable<OrdersApiService.Order> orders)
    {
        var promptBuilder = new StringBuilder();
        promptBuilder.AppendLine("Aqui está um resumo dos pedidos recentes:");

        foreach (var order in orders)
        {
            promptBuilder.AppendLine($"Pedido {order.OrderNumber}, Status: {order.Status}, Aprovação do Gerente Necessária: {order.NeedsManagerApproval}");
        }

        promptBuilder.AppendLine("Como posso ajudar com esses pedidos?");
        return promptBuilder.ToString();
    }


    public string CreatePromptForOrderDetails(OrdersApiService.Order order, string userQuestion)
    {
        if (order == null)
        {
            return "Pedido não encontrado.";
        }

        var orderDetails = new StringBuilder($"Pedido {order.OrderNumber} encontrado.\n");

        foreach (var property in typeof(OrdersApiService.Order).GetProperties())
        {
            var value = property.GetValue(order);

            if (value != null)
            {
                if (property.Name == "Status" && value is int statusValue)
                {
                    string statusDescription = MapStatusToDescription(statusValue);
                    orderDetails.AppendLine($"Status: {statusDescription}");
                }
                else if (property.Name == "EmployeeId" && value is int employeeIdValue)
                {
                    string employeeName = MapEmployeeIdToName(employeeIdValue);
                    orderDetails.AppendLine($"Funcionário Responsável: {employeeName}");
                }
                else
                {
                    orderDetails.AppendLine($"{property.Name}: {value}");
                }
            }
        }

        orderDetails.AppendLine($"Pergunta do usuário: '{userQuestion}'");

        return orderDetails.ToString();
    }

    internal Task<string> GetResponseAsync(string prompt)
    {
        throw new NotImplementedException();
    }

    // Tornando o GetChatGptResponse Sync
    public string GetChatGptResponseSync(string prompt)
    {
        if (_chatGptClient == null)
        {
            Log.Error("ChatGptClient is not initialized.");
            return "Service Unavailable";
        }
        try
        {
            var response = _chatGptClient.GetResponseAsync(prompt).GetAwaiter().GetResult();
            Log.Information($"Recebido pelo prompt: {prompt}");
            return response;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao obter resposta do ChatGPT");
            return "Error getting response";
        }
    }

    public async Task<string> GetChatGptResponse(string prompt)
    {
        return await _chatGptClient.GetResponseAsync(prompt);
    }

    private string MapStatusToDescription(int status)
    {
        return status switch
        {
            1 => "1 - Draft",
            2 => "2 - Submitted",
            3 => "3 - Approved",
            4 => "4 - Validated",
            5 => "5 - Waiting",
            6 => "6 - Signed",
            7 => "7 - Closed",
            8 => "8 - Rejected",
            9 => "9 - Discarded",
            _ => $"{status} - Unknown Status"
        };
    }

    private string MapEmployeeIdToName(int employeeId)
    {
        return employeeId switch
        {
            83 => "83 - William M. Whatley",
            82 => "82 - William M. Jones",
            81 => "81 - Wayne G. Lape",
            80 => "80 - Vivienne R. Mcelligott",
            79 => "79 - Virginia L. Lord",
            78 => "78 - Victor L. Woo",
            77 => "77 - Tommy D. Talbot",
            76 => "76 - Tina J. Camacho",
            75 => "75 - Theodore S. Schaal",
            74 => "74 - Tara K. Oneil",
            73 => "73 - Stevie Wonder",
            72 => "72 - Salvatore L. Hardwick",
            71 => "71 - Ruth J. Mckeever",
            70 => "70 - Roy S. Hightower",
            69 => "69 - Roosevelt D. Pennington",
            68 => "68 - Ronald C. Bainter",
            67 => "67 - Ronald A. Chilson",
            66 => "66 - Robert K. Smith",
            65 => "65 - Rex M. Hebert",
            64 => "64 - Phyllis R. Bowler",
            60 => "60 - Patricia P.Wesley",
            _ => $"Employee ID: {employeeId} - Unknown Employee"
        };
    }
}
