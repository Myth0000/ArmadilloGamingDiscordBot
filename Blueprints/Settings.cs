using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using Discord.WebSocket;

namespace ArmadilloGamingDiscordBot.Blueprints
{
    /// <summary>
    /// Contains settings for modifying the bot's functionalities in discord.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Settings
    {
        public int ExpGainCooldown { get; set; } = 8;
        public ulong LevelUpMessagesChannelId { get; set; }
    }
}
