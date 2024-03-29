﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;


namespace ArmadilloGamingDiscordBot.Blueprints
{
    [BsonIgnoreExtraElements]
    public class VirtualItem
    {
        /// <summary>
        /// An unique Id exclusive to this object.
        /// </summary>
        public long Id { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        public string Name { get; set; }
        public string Description { get; set; }
        public string EmoteId { get; set; }
        public string ImageUrl { get; set; }
        public string Obtaining { get; set; }
        public string Rarity { get; set; } // COMMON, RARE, UNIQUE, LEGENDARY, MYTHIC
    }
}
