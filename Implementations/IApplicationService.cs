using OutSystems.ExternalLibraries.SDK;

[OSInterface(Description = "Agenor", IconResourceName = "AgenorAI.resources.qAgenorTransp.png", Name = "AgenorAI")]
public interface IApplicationService
{
    string GetChatGptResponseSync(string prompt);
    string GetChatGptResponseForOrderDetailsSync(string userQuestion);
    void TestConnection();
}