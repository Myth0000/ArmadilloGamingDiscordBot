using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace ArmadilloGamingDiscordBot.Blueprints
{
    [BsonIgnoreExtraElements]
    public class ShopItem
    {
        public string Name { get; set; } = "none";
        public string Description { get; set; } = "none";
        public int Price { get; set; } = 0;

        /// <summary>
        /// The type of item that is listed on the shop. ex. Virtual Item
        /// </summary>
        public string ItemType { get; set; } = "none";
    }
}
