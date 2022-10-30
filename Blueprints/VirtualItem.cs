using System;
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
        public string Name { get; set; }
        public string Description { get; set; }
        public string EmoteId { get; set; }

    }
}
