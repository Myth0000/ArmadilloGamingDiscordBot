using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;


namespace ArmadilloGamingDiscordBot
{

    /// <summary>
    /// Stores random things such as variables, etc., that can be used throughout the application.
    /// </summary>
    public static class Storage
    {
        // These 3 props are used throughout the application
        public static string MongoDBConnectionString { get { return GetObject(configObjects.MainDatabaseConnectionString); } }
        public static string DiscordBotToken { get { return GetObject(configObjects.MainBotToken); } }
        public static ulong DiscordServerId { get { return ArmadilloGamingGuildId; } }

        // stuff
        public static ulong ArmadilloGamingGuildId { get { return 892701424218148905; } }
        public static ulong DiscordBotTestGuildId { get { return 1036338309410082977; } }

        // Emotes
        public static string ArmadilloCoinEmoteId { get { return "<:ArmadilloCoin:1036766409671327825>"; } }

        public enum configObjects { MainBotToken, TestBotToken, MainDatabaseConnectionString, TestBotDatabaseConnectionString }
        public static string GetObject(configObjects configObject)
        {
            Config config = JsonSerializer.Deserialize<Config>(File.ReadAllText("config.json"));
            
            switch(configObject)
            {
                case configObjects.MainBotToken:
                    return config.MainBotToken;
                case configObjects.TestBotToken:
                    return config.TestBotToken;
                case configObjects.TestBotDatabaseConnectionString:
                    return config.TestDatabaseConnectionString;
                case configObjects.MainDatabaseConnectionString:
                    return config.MainDatabaseConnectionString;
            }

            return null;
        }
    }

    public class Config
    {
        public string MainBotToken { get; set; }
        public string TestBotToken { get; set; }
        public string MainDatabaseConnectionString { get; set; }
        public string TestDatabaseConnectionString { get; set; }
    }
}
