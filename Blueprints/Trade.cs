using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;
using MongoDB.Bson.Serialization.Attributes;
using ArmadilloGamingDiscordBot.Blueprints;

namespace ArmadilloGamingDiscordBot.Blueprints
{
    /// <summary>
    /// A blueprint for when a trade occurs between two players.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Trade
    {
        public ulong TradeThreadChannelId { get; set; }
        public Trader Trader1 { get; set; }
        public Trader Trader2 { get; set; }
    }

    public class Trader
    {
        public ulong GuildUserId { get; set; }
        public ulong TradeMenuMessageId { get; set; }
        public List<VirtualItem> VirtualItems { get; set; } = new List<VirtualItem>();
        public bool TradeAccepted { get; set; } = false;
    }
}
