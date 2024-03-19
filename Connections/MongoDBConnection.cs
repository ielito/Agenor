using MongoDB.Driver;
using MongoDB.Bson;
using System.Threading.Tasks;
using Serilog;
using System;

public class MongoDBConnection
{
    private readonly MongoClient? _client;
    private readonly string _databaseName = "ielitoLab";

    // Construtor sem parametros
    public MongoDBConnection() {}
    
    public MongoDBConnection(string connectionString)
    {

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("A string de conexão é necessária.", nameof(connectionString));

        var settings = MongoClientSettings.FromConnectionString(connectionString);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        _client = new MongoClient(settings);
    }

    public void TestConnection()
    {
        try
        {
            if (_client != null)
            {
                var result = _client.GetDatabase(_databaseName).RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                Console.WriteLine("Conexão com o MongoDB bem-sucedida.");
                Log.Information("Sucesso na conexão com o MongoDB");
            }
            else
            {
                Log.Warning("O cliente MongoDB não foi inicializado.");
            }
        }
        catch (MongoException mongoEx)
        {
            Log.Error($"Erro ao conectar no MongoDB: {mongoEx.Message}");
        }
        catch (Exception ex)
        {
            Log.Error($"Erro ao testar conexão com o MongoDB: {ex.Message}");
        }
    }

    public async Task TestConnectionAsync()
    {
        if (_client == null)
        {
            Log.Warning("O cliente MongoDB não foi inicializado para teste assíncrono.");
            return;
        }

        try
        {
            var result = await _client.GetDatabase(_databaseName).RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1));
            Log.Information("Conexão assíncrona com o MongoDB bem-sucedida.");
        }
        catch (MongoException mongoEx)
        {
            Log.Error($"Erro MongoDB: {mongoEx.Message}");
        }
        catch (Exception ex)
        {
            Log.Error($"Erro ao testar conexão assíncrona com o MongoDB: {ex.Message}");
        }
    }
}
