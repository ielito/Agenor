using MongoDB.Driver;
using MongoDB.Bson;
using System.Threading.Tasks;
using Serilog;
using System;

public class MongoDBConnection
{
    private readonly MongoClient _client;
    private readonly string _databaseName = "ielitoLab";

    public MongoDBConnection()
    {
    }

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

    public async Task TestConnectionAsync()
    {
        try
        {
            var result = await _client.GetDatabase(_databaseName).RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1));
            Log.Information("Pinged your deployment. You successfully connected to MOngoDB!");
        }
        catch (MongoException mongoEx)
        {
            Log.Error(mongoEx, "MongoDB error");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Generic Error - I know, but it is what it is");
        }
    }
}
