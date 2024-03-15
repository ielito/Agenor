using OutSystems.ExternalLibraries.SDK;
using System.Threading.Tasks;

[OSInterface]
public interface IApplicationService
{
    Task<string> GetChatGptResponse(string prompt);
    Task<string> GetChatGptResponseForOrderDetails(string userQuestion);
    Task TestConnectionAsync();
}
