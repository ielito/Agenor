using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using static OrdersApiService;
using Serilog;

public class MongoDbService
{
    private readonly IMongoDatabase _database;
    private readonly string _collectionName = "orders";
    private readonly MongoClient _client;

    public MongoDbService(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("A string de conexão do MongoDB é inválida.");
        }

        _client = new MongoClient(connectionString);
        _database = _client.GetDatabase("Orders");

        BsonClassMap.RegisterClassMap<MongoOrder>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true); // Ignora campos extras como '_id'
        });

    }

    public void InsertOrdersIntoMongoDb(IEnumerable<OrdersApiService.Order> orders)
    {
        var database = _client.GetDatabase("Orders");
        var collection = database.GetCollection<OrdersApiService.Order>("orders");

        collection.InsertMany(orders);
    }

    public IEnumerable<OrdersApiService.Order> GetOrdersFromMongoDb(int pageNumber = 1, int pageSize = 10)
    {
        var collection = _database.GetCollection<OrdersApiService.Order>(_collectionName);
        return collection.Find(_ => true)
                         .Skip((pageNumber - 1) * pageSize)
                         .Limit(pageSize)
                         .ToList();
    }

    public MongoOrder? GetOrderDetails(string orderNumber)
    {
        var database = _client.GetDatabase("Orders");
        var collection = _database.GetCollection<MongoOrder>(_collectionName);
        var filter = Builders<MongoOrder>.Filter.Eq("OrderNumber", orderNumber);
        var order = collection.Find(filter).FirstOrDefault();

        if (order != null)
        {
            Console.WriteLine($"Pedido encontrado: {order.OrderNumber}");
            Log.Information($"Pedido Encontrado:{order.OrderNumber}");
        }
        else
        {
            Console.WriteLine("Pedido não encontrado.");
            Log.Information($"Pedido não encontrado.");
        }

        return order;
    }

    public void TestConnection()
    {
        try
        {
            var database = _client.GetDatabase("Orders");
            var command = new BsonDocument { { "ping", 1 } };
            var result = database.RunCommand<BsonDocument>(command);
            Console.WriteLine("Conexão com o MongoDB: Sucesso.");
            Log.Information("Conexão com o MongoDB realizado com sucesso");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao conectar com o MongoDB: {ex.Message}");
            Log.Information($"Erro ao conectar com o MongoDB: {ex.Message}");
        }
    }

    public void InsertOrUpdateOrders(IEnumerable<Order> orders)
    {
        var database = _client.GetDatabase("Orders");
        var collection = database.GetCollection<Order>("orders");

        foreach (var order in orders)
        {
            var filter = Builders<Order>.Filter.Eq(o => o.OrderNumber, order.OrderNumber);
            var update = Builders<Order>.Update
                .Set(o => o.Status, order.Status)
                .Set(o => o.NeedsManagerApproval, order.NeedsManagerApproval);

            collection.UpdateOne(filter, update, new UpdateOptions { IsUpsert = true });
        }
    }
}
