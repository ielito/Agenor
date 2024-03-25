using System;
namespace AgenorAI.Interfaces
{
	public class getInfo
	{
		private string _mongoDbConnectionString;
		private string _openAiApiKey;

		public getInfo(string mongoDbConnectionString, string openAiApiKey)
		{
			_mongoDbConnectionString = mongoDbConnectionString;
			_openAiApiKey = openAiApiKey;
		}
		
	}
}

