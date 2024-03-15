using System.Text.RegularExpressions;
using MongoDB.Driver;
using static OrdersApiService;

public class ApplicationService
{
    private string _lastOrderNumber;

    private MongoDbService _mongoDbService;
    private ChatGptService _chatGptService;
    private OrdersApiService _ordersApiService;

    public ApplicationService(string mongoDbConnectionString, string openAiApiKey)
    {
        _mongoDbService = new MongoDbService(mongoDbConnectionString);
        _chatGptService = new ChatGptService(openAiApiKey);
        _ordersApiService = new OrdersApiService();
    }

    public void TestDatabaseConnection()
    {
        _mongoDbService.TestConnection();
    }

    public async Task<string> GetChatGptResponse(string prompt)
    {
        return await _chatGptService.GetResponseAsync(prompt);
    }

    //Refactor para Sync do GetChatGptResponseForOrderDetails
    public string GetChatGptResponseForOrderDetailsSync(string userQuestion)
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

        var task = GetChatGptResponseForOrderDetails(userQuestion);
        task.Wait();
        return task.Result;

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

}