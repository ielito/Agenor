public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Por favor, insira a string de conexão do MongoDB:");
        string? mongoDbConnectionString = Console.ReadLine() ?? throw new InvalidOperationException("String de conexão do MongoDB não fornecida.");
        if (string.IsNullOrEmpty(mongoDbConnectionString)) throw new InvalidOperationException("String de conexão do MongoDB não fornecida.");


        Console.WriteLine("Por favor, insira a chave da API do OpenAI:");
        string? openAiApiKey = Console.ReadLine() ?? throw new InvalidOperationException("Chave da API do OpenAI não fornecida.");
        if (string.IsNullOrEmpty(openAiApiKey)) throw new InvalidOperationException("Chave da API do OpenAI não fornecida.");

        ApplicationService appService = new ApplicationService(mongoDbConnectionString, openAiApiKey);

        // Testar a conexão com o banco de dados
        appService.TestDatabaseConnection();

        // Atualizar MongoDB com dados da API
        await appService.UpdateMongoDbWithApiData();

        while (true)
        {
            Console.WriteLine("Por favor, digite sua pergunta ou 'SAIR' para encerrar:");
            string userQuestion = Console.ReadLine();

            if (string.IsNullOrEmpty(userQuestion))
            {
                Console.WriteLine("Entrada inválida. Por favor, tente novamente.");
                continue;
            }

            if (userQuestion.Trim().ToUpper() == "SAIR")
            {
                Console.WriteLine("Encerrando o programa.");
                break;
            }

            string response = await appService.GetChatGptResponseForOrderDetails(userQuestion);
            Console.WriteLine(response);
        }
    }
}