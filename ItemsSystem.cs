using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using ArmadilloGamingDiscordBot.Blueprints;

namespace ArmadilloGamingDiscordBot
{
    public static class ItemsSystem
    {

        /// <summary>
        /// Returns an array of all the items the user has.
        /// </summary>
        public static List<Item> GetUserInventory(MongoClient mongoClient, ulong userId)
        {
            var userCollection = mongoClient.GetDatabase("UserDatabase").GetCollection<BsonDocument>("User");
            var userFilter = Builders<BsonDocument>.Filter.Eq("UserId", userId);

            return BsonSerializer.Deserialize<User>(userCollection.Find<BsonDocument>(userFilter).First()).Inventory; 
        }


        public static Item ConvertEmoteIdToItem(string emoteId, string emoteDescription)
        {
            string emoteName = emoteId.Split(':')[1];

            return new Item()
            {
                Name = emoteName,
                Description = emoteDescription,
                EmoteId = emoteId
            };
        }


        public static void AddItemToDatabase(MongoClient mongoClient, Item item)
        {
            var itemsCollection = mongoClient.GetDatabase("ItemsDatabase").GetCollection<BsonDocument>("Items");
            itemsCollection.InsertOne(item.ToBsonDocument());
        }


        public static Item GetItemFromDatabase(MongoClient mongoClient, string itemName)
        {
            var itemsCollection = mongoClient.GetDatabase("ItemsDatabase").GetCollection<BsonDocument>("Items");
            var itemFilter = Builders<BsonDocument>.Filter.Eq("Name", itemName);

            return BsonSerializer.Deserialize<Item>(itemsCollection.Find<BsonDocument>(itemFilter).First());
        }


        public static void AddItemToUserInventory(MongoClient mongoClient, Item item, ulong userId)
        {
            var userCollection = mongoClient.GetDatabase("UserDatabase").GetCollection<BsonDocument>("User");
            var userFilter = Builders<BsonDocument>.Filter.Eq("UserId", userId);
            User user = BsonSerializer.Deserialize<User>(userCollection.Find<BsonDocument>(userFilter).First());

            // Adds a new item to the inventory

            if(user.Inventory == null) 
            {
                user.Inventory = new List<Item>() { item };
            }
            else
            {
                user.Inventory.Add(item);
            }
           
            // updates inventory in the database
            var updateInventory = Builders<BsonDocument>.Update.Set("Inventory", user.Inventory);
            userCollection.UpdateOne(userFilter, updateInventory);           
        }
    }
}
