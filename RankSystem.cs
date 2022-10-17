using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace ArmadilloGamingDiscordBot
{
    public static class RankSystem
    {

        
        public static void displayRankProperties(Rank rank)
        {
            Console.WriteLine($"Level: {rank.Level}\nExp: {rank.CurrentExp}/{rank.MaxExp}\nTotal Exp: {rank.TotalExp}");
        }




        /// <summary>
        /// Updates the rank to make sure level ups, etc. are happening.
        /// </summary>
        public static void UpdateRankOnMessageSent(MongoClient mongoClient, ulong UserId)
        {

            //var user = new User(UserId).ToBsonDocument<User>();
            var userFilter = Builders<BsonDocument>.Filter.Eq("UserId", UserId);

            var userBsonDoc = mongoClient.GetDatabase("UserDatabase").GetCollection<BsonDocument>("User").Find(userFilter).First();
            User user = BsonSerializer.Deserialize<User>(userBsonDoc);


            Console.WriteLine(user);
            Console.WriteLine("Message Recieved!");
        }




        /// <summary>
        /// Adds the user to the UserDatabase.
        /// </summary>
        public static void CreateNewUser(MongoClient mongoClient, ulong UserId)
        {
            var user = new User(UserId).ToBsonDocument<User>();

            var usersCollection = mongoClient.GetDatabase("UserDatabase").GetCollection<BsonDocument>("User");
            usersCollection.InsertOne(user);
        }



    }




    public class Rank
    {
        /// <summary>
        /// The current level.
        /// </summary>
        public int Level { get; set; } = 0;

        /// <summary>
        /// The amount of Exp in possession.
        /// </summary>
        public int CurrentExp { get; set; } = 0;

        /// <summary>
        /// The maximum amount of Exp needed to level up to the next level.
        /// </summary>
        public int MaxExp { get; set; } = 10;

        /// <summary>
        /// The total amount of Exp in possession since level 0.
        /// </summary>
        public int TotalExp { get; set; } = 0;
    }
}
