using MongoDB.Driver;
using MongoDB.Bson;
using System;

public class MongoDBConnection
{
    private readonly MongoClient _client;

    public MongoDBConnection(string connectionString)
    {
        var settings = MongoClientSettings.FromConnectionString(connectionString);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        _client = new MongoClient(settings);
    }

    public void TestConnection()
    {
        try
        {
            var result = _client.GetDatabase("ielitoLab").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
            Console.WriteLine("Pinged your deployment. You successfully connected to MongoDB!");
        }
        catch (MongoException mongoEx)
        {
            Console.WriteLine($"Erro de MongoDB: {mongoEx.Message}");
            // Adicione aqui o log
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro geral: {ex.Message}");
            // Adicione aqui o log
        }
    }
}
