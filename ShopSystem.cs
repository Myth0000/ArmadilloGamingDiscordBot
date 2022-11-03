using ArmadilloGamingDiscordBot.Blueprints;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace ArmadilloGamingDiscordBot
{
    public static class ShopSystem
    {
        public static void AddItemToShop(MongoClient mongoClient, ShopItem item)
        {
            var shopCollection = mongoClient.GetDatabase("ShopDatabase").GetCollection<BsonDocument>("Shop");

            shopCollection.InsertOne(item.ToBsonDocument());
        }




        public static ShopItem[] GetAllItemsFromShop(MongoClient mongoClient)
        {
            var shopCollection = mongoClient.GetDatabase("ShopDatabase").GetCollection<BsonDocument>("Shop");
            var shopItemBsonDocuments = shopCollection.Find<BsonDocument>(new BsonDocument()).ToList();
            List<ShopItem> shopItemsList = new();

            foreach (var document in shopItemBsonDocuments)
            {
                shopItemsList.Add(BsonSerializer.Deserialize<ShopItem>(document));
            }
            return shopItemsList.ToArray();
        }




   
    }
}
