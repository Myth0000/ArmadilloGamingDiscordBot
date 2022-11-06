using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmadilloGamingDiscordBot
{
    /// <summary>
    /// Stores random things such as variables, etc., that can be used throughout the application.
    /// </summary>
    public static class Storage
    {
        // MongoDB connection strings
        public static string MongoDBConnectionString { get { return TestBotDatabaseConnectionString; } }
        public static string ArmadillGamingDatabaseConnectionString { get { return "mongodb+srv://Myth0000:JhgZ5shGWcxj3kEj@usercluster.djfruor.mongodb.net/?retryWrites=true&w=majority"; } }
        public static string TestBotDatabaseConnectionString { get { return "mongodb+srv://Myth:JyCgBey037w7bfMt@cluster0.clsz5ty.mongodb.net/?retryWrites=true&w=majority"; } }

        // Guild IDs
        public static ulong ArmadilloGamingGuildId { get { return 892701424218148905; } }
        public static ulong DiscordBotTestGuildId { get { return 810214297207570452; } }

        // Emotes
        public static string ArmadilloCoinEmoteId { get { return "<:ArmadilloCoin:1036766409671327825>"; } }
    }
}
