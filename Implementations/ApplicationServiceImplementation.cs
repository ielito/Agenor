﻿public class ApplicationServiceImplementation : IApplicationService
{
    private readonly ApplicationService _appService;
    private readonly MongoDBConnection _mongoDbConnection;

    public ApplicationServiceImplementation(string mongoDbConnectionString, string openAiApiKey)
    {
        _appService = new ApplicationService(mongoDbConnectionString, openAiApiKey);
        _mongoDbConnection = new MongoDBConnection();
    }

    public ApplicationServiceImplementation()
    {
    }

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


}
