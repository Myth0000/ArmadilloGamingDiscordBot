﻿using System;
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
    public static class VirtualItemSystem
    {

        /// <summary>
        /// Returns an array of all the items the user has.
        /// </summary>
        public static List<VirtualItem> GetUserInventory(MongoClient mongoClient, ulong userId)
        {
            var userCollection = mongoClient.GetDatabase("UserDatabase").GetCollection<BsonDocument>("User");
            var userFilter = Builders<BsonDocument>.Filter.Eq("UserId", userId);

            return BsonSerializer.Deserialize<User>(userCollection.Find<BsonDocument>(userFilter).First()).Inventory; 
        }




        public static VirtualItem ConvertEmoteIdToItem(string emoteId, string imageUrl, string obtaining, string rarity, string emoteDescription)
        {
            string emoteName = emoteId.Split(':')[1];

            return new VirtualItem()
            {
                Name = emoteName,
                Description = emoteDescription,
                EmoteId = emoteId,
                ImageUrl = imageUrl,
                Obtaining = obtaining,
                Rarity = rarity,
            };
        }




        public static void AddItemToDatabase(MongoClient mongoClient, VirtualItem item)
        {
            var itemsCollection = mongoClient.GetDatabase("VirtualItemDatabase").GetCollection<BsonDocument>("VirtualItem");
            itemsCollection.InsertOne(item.ToBsonDocument());
        }




        public static VirtualItem GetItemFromDatabase(MongoClient mongoClient, string itemName)
        {
            var itemsCollection = mongoClient.GetDatabase("VirtualItemDatabase").GetCollection<BsonDocument>("VirtualItem");
            var itemFilter = Builders<BsonDocument>.Filter.Eq("Name", itemName);

            return BsonSerializer.Deserialize<VirtualItem>(itemsCollection.Find<BsonDocument>(itemFilter).First());
        }




        public static VirtualItem[] GetAllItemsFromDatabase(MongoClient mongoClient)
        {
            var itemsCollection = mongoClient.GetDatabase("VirtualItemDatabase").GetCollection<BsonDocument>("VirtualItem");
            var itemsBsonDocumentList = itemsCollection.Find<BsonDocument>(new BsonDocument()).ToList();
            List<VirtualItem> virtualItemsList = new();

            foreach(var document in itemsBsonDocumentList)
            {
                VirtualItem item = BsonSerializer.Deserialize<VirtualItem>(document);
                virtualItemsList.Add(item);
            }

            return virtualItemsList.ToArray();
        }




        public static void AddItemToUserInventory(MongoClient mongoClient, VirtualItem item, ulong userId)
        {
            var userCollection = mongoClient.GetDatabase("UserDatabase").GetCollection<BsonDocument>("User");
            var userFilter = Builders<BsonDocument>.Filter.Eq("UserId", userId);
            User user = BsonSerializer.Deserialize<User>(userCollection.Find<BsonDocument>(userFilter).First());

            // Adds a new item to the inventory

            if(user.Inventory == null) 
            {
                user.Inventory = new List<VirtualItem>() { item };
            }
            else
            {
                user.Inventory.Add(item);
            }
           
            // updates inventory in the database
            var updateInventory = Builders<BsonDocument>.Update.Set("Inventory", user.Inventory);
            userCollection.UpdateOne(userFilter, updateInventory);           
        }




        public static void RemoveItemFromUserInventory(MongoClient mongoClient, VirtualItem item, ulong userId)
        {
            var userCollection = mongoClient.GetDatabase("UserDatabase").GetCollection<BsonDocument>("User");
            var userFilter = Builders<BsonDocument>.Filter.Eq("UserId", userId);
            User user = BsonSerializer.Deserialize<User>(userCollection.Find<BsonDocument>(userFilter).First());

            // Removes the item from the inventory

            if ((user.Inventory != null))
            {
                if (user.Inventory.Count != 0)
                {
                    user.Inventory.RemoveAt(user.Inventory.FindIndex(_item => _item.Id == item.Id));
                }
            }

            // updates inventory in the database
            var updateInventory = Builders<BsonDocument>.Update.Set("Inventory", user.Inventory);
            userCollection.UpdateOne(userFilter, updateInventory);
        }




        public static VirtualItem GetRandomItemWithObtaining(MongoClient mongoClient, string obtaining)
        {
            var virtualItemCollection = mongoClient.GetDatabase("VirtualItemDatabase").GetCollection<BsonDocument>("VirtualItem");
            var obtainingFilter = Builders<BsonDocument>.Filter.Eq("Obtaining", obtaining);
            List<VirtualItem> virtualItemsList = new();

            List<BsonDocument> itemsBsonDocumentList = virtualItemCollection.Find<BsonDocument>(obtainingFilter).ToList();
            var randomBsonItem = itemsBsonDocumentList[new Random().Next(0, itemsBsonDocumentList.Count)];

            VirtualItem virtualItem = BsonSerializer.Deserialize<VirtualItem>(randomBsonItem);
            virtualItem.Id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); // creates a unique id for each item 

            return virtualItem;
        }
    



    
    
    
    }
}
