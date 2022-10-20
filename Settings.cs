using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace ArmadilloGamingDiscordBot
{
    /// <summary>
    /// Contains settings for modifying the bot's functionalities in discord.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Settings
    {
        public int ExpGainCooldown { get; set; }
    }
}
