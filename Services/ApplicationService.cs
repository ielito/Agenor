using System.Text.RegularExpressions;
using MongoDB.Driver;
using static OrdersApiService;
using Serilog;

public class ApplicationService
{
    private string? _lastOrderNumber;

    private MongoDbService? _mongoDbService;
    private ChatGptService? _chatGptService;
    private OrdersApiService? _ordersApiService;
    private object _chatGptClient;

    public ApplicationService(string mongoDbConnectionString, string openAiApiKey)
    {
        Log.Information("Conectando no MongoDB");
        _mongoDbService = new MongoDbService(mongoDbConnectionString);
        _chatGptService = new ChatGptService(openAiApiKey);
        _ordersApiService = new OrdersApiService();
    }


    public void TestDatabaseConnection()
    {
        if (_mongoDbService != null)
        {
            _mongoDbService.TestConnection();
        }
        else
        {
            Log.Information("Conectado com sucesso");
        }
        
    }

    public async Task<string> GetChatGptResponse(string prompt)
    {
        if (_chatGptService != null)
        {
            return await _chatGptService.GetResponseAsync(prompt);
        }
        else
        {
            Log.Information("Sem sucesso");
            return "Sem sucesso";
        }
        
    }

    public string GetChatGptResponseForOrderDetailsSync(string userQuestion)
    {
        string orderNumber = ExtractOrderNumber(userQuestion);

        if (string.IsNullOrEmpty(orderNumber) && !string.IsNullOrEmpty(_lastOrderNumber))
        {
            orderNumber = _lastOrderNumber;
        }
        else if (string.IsNullOrEmpty(orderNumber))
        {
            return "Número do pedido não fornecido.";
        }

        if (_mongoDbService == null)
        {
            Log.Error("MongoDbService is null.");
            return "Service Unavailable";
        }

        var mongoOrder = _mongoDbService.GetOrderDetails(orderNumber);

        if (mongoOrder == null)
        {
            return "Pedido não encontrado.";
        }

        if (_chatGptService == null)
        {
            Log.Error("ChatGptService is null.");
            return "Service Unavailable";
        }

        return _chatGptService.GetChatGptResponseSync(CreatePromptForOrderDetails(mongoOrder, userQuestion));
    }

    private string CreatePromptForOrderDetails(MongoOrder mongoOrder, string userQuestion)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetChatGptResponseForOrderDetails(string userQuestion)
    {
        string orderNumber = ExtractOrderNumber(userQuestion);

        if (string.IsNullOrEmpty(orderNumber) && !string.IsNullOrEmpty(_lastOrderNumber))
        {
            orderNumber = _lastOrderNumber;
        }
        else
        {
            _lastOrderNumber = orderNumber;
        }

        var mongoOrder = _mongoDbService.GetOrderDetails(orderNumber);

        if (mongoOrder == null)
        {
            return "Pedido não encontrado.";
        }

        // Converte MongoOrder para Order
        Order order = ConvertMongoOrderToOrder(mongoOrder);

        string prompt = _chatGptService.CreatePromptForOrderDetails(order, userQuestion);
        return await _chatGptService.GetChatGptResponse(prompt);
    }

    private Order ConvertMongoOrderToOrder(MongoOrder mongoOrder)
    {
        return new Order
        {
            OrderNumber = mongoOrder.OrderNumber,
            Status = mongoOrder.Status,
            NeedsManagerApproval = mongoOrder.NeedsManagerApproval
            // Copie outros campos conforme necessário
        };
    }

    private string ExtractOrderNumber(string question)
    {
        var regex = new Regex(@"(ord\d+)", RegexOptions.IgnoreCase);
        var match = regex.Match(question);

        if (match.Success)
        {
            // Novo número de pedido encontrado, atualiza _lastOrderNumber
            _lastOrderNumber = match.Value.Trim();
            return _lastOrderNumber;
        }

        // Retorna o último número de pedido se nenhum novo número for encontrado
        return _lastOrderNumber;
    }

    public void InsertOrderIntoMongoDb(OrdersApiService.Order order)
    {
        _mongoDbService.InsertOrdersIntoMongoDb(new List<OrdersApiService.Order> { order });
    }

    public async Task UpdateMongoDbWithApiData()
    {
        var ordersFromApi = await GetOrdersFromApi(); // Chamada correta para o método implementado
        _mongoDbService.InsertOrUpdateOrders(ordersFromApi); // Inserir ou atualizar os dados no MongoDB
    }

    public async Task<List<Order>> GetOrdersFromApi()
    {
        return await _ordersApiService.GetOrdersFromApi();
    }

    internal string GetChatGptResponseSync(string prompt)
    {
        throw new NotImplementedException();
    }
}