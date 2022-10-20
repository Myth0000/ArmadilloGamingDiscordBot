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
        public static string ArmadillGamingDatabaseConnectionString { get { return "mongodb+srv://Myth0000:JhgZ5shGWcxj3kEj@usercluster.djfruor.mongodb.net/?retryWrites=true&w=majority"; } }
        public static string TestBotDatabaseConnectionString { get { return "mongodb+srv://Myth:JyCgBey037w7bfMt@cluster0.clsz5ty.mongodb.net/?retryWrites=true&w=majority"; } }
    }
}
