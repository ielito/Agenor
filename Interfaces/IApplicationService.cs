using OutSystems.ExternalLibraries.SDK;

[OSInterface]
public interface IApplicationService
{
    string GetChatGptResponseSync(string prompt);
    string GetChatGptResponseForOrderDetailsSync(string userQuestion);
    void TestConnection();
}
