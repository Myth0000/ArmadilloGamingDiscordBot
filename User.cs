using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ArmadilloGamingDiscordBot.Blueprints;

namespace ArmadilloGamingDiscordBot
{
    [BsonIgnoreExtraElements]
    public class User
    {
        public ulong UserId { get; set; }
        public Rank Rank { get; set; } = new Rank();
        public int ArmadilloCoin { get; set; } = 0;
        public List<Item> Inventory { get; set; } = new();

        public User(ulong userId)
        {
            UserId = userId;
        }
    }
}
