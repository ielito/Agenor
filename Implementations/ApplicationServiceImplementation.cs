using AgenorAI.Interfaces;

public class ApplicationServiceImplementation : IApplicationService
{
    private readonly ApplicationService _appService;
    private readonly MongoDBConnection _mongoDbConnection;

    public ApplicationServiceImplementation(getInfo config)
    {
        // Utilize as propriedades de config para inicializar _appService e _mongoDbConnection
        _appService = new ApplicationService(config.MongoDbConnectionString, config.OpenAiApiKey);
        _mongoDbConnection = new MongoDBConnection(config.MongoDbConnectionString);
    }

    public ApplicationServiceImplementation() { }

    public async Task<string> GetChatGptResponse(string prompt)
    {
        return await _appService.GetChatGptResponse(prompt);
    }

    public async Task<string> GetChatGptResponseForOrderDetails(string userQuestion)
    {
        return await _appService.GetChatGptResponseForOrderDetails(userQuestion);
    }

    public async Task TestConnectionAsync()
    {
        await _mongoDbConnection.TestConnectionAsync();
    }

    // Sync para a OutSystems reconhecer

    public string GetChatGptResponseSync(string prompt)
    {
        return _appService.GetChatGptResponseSync(prompt);
    }

    public string GetChatGptResponseForOrderDetailsSync(string userQuestion)
    {
        return _appService.GetChatGptResponseForOrderDetailsSync(userQuestion);
    }

    public void TestConnection()
    {
        _mongoDbConnection.TestConnection();
    }

    string IApplicationService.ApplicationServiceImplementation()
    {
        throw new NotImplementedException();
    }

    public string getInfo()
    {
        throw new NotImplementedException();
    }
}
