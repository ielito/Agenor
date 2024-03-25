using System;
using AgenorAI.Interfaces;
using static AgenorAI.Interfaces.getInfo;

namespace AgenorAI.Interfaces
{
    public class getInfo
    {
        public string MongoDbConn { get; private set; }
        public string ApiKey { get; private set; }
        public string MongoDbConnectionString { get; internal set; }
        public string OpenAiApiKey { get; internal set; }

        public getInfo(string mongoDbConnectionString, string openAiApiKey)
        {
            MongoDbConn = mongoDbConnectionString;
            ApiKey = openAiApiKey;
        }
    }

}

