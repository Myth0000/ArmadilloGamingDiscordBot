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
        public static void UpdateRankOnMessageSent(MongoClient mongoClient, SocketMessage message)
        {
            GiveUserExp(mongoClient, message.Author.Id);

            // Level up code below
            var userFilter = Builders<BsonDocument>.Filter.Eq("UserId", message.Author.Id);

            var userCollection = mongoClient.GetDatabase("UserDatabase").GetCollection<BsonDocument>("User");
            Rank userRank = BsonSerializer.Deserialize<User>(userCollection.Find<BsonDocument>(userFilter).First()).Rank;

            if(userRank.CurrentExp >= userRank.MaxExp)
            {
                // Cexp -= Mexp

                var updateCurrentExp = Builders<BsonDocument>.Update.Set("Rank.CurrentExp", (userRank.CurrentExp - userRank.MaxExp));
                var updateMaxExp = Builders<BsonDocument>.Update.Set("Rank.MaxExp", Math.Floor(userRank.MaxExp * 1.1));
                var updateLevel = Builders<BsonDocument>.Update.Set("Rank.Level", ++userRank.Level);

                userCollection.UpdateOne(userFilter, updateCurrentExp);
                userCollection.UpdateOne(userFilter, updateMaxExp);
                userCollection.UpdateOne(userFilter, updateLevel);

                message.Channel.SendMessageAsync($"{GuildEmotes.armadillo} {message.Author.Mention} has leveled up to level {userRank.Level}!");
            }

        }




        /// <summary>
        /// Adds the user to the UserDatabase.
        /// </summary>
        public static void CreateNewUser(MongoClient mongoClient, ulong userId)
        {
            var user = new User(userId).ToBsonDocument<User>();

            var usersCollection = mongoClient.GetDatabase("UserDatabase").GetCollection<BsonDocument>("User");
            usersCollection.InsertOne(user);
        }




        /// <summary>
        /// Gives exp to the User.
        /// </summary>
        public static void GiveUserExp(MongoClient mongoClient, ulong userId)
        {
            var userCollection = mongoClient.GetDatabase("UserDatabase").GetCollection<BsonDocument>("User");
            var userFilter = Builders<BsonDocument>.Filter.Eq("UserId", userId);
            User user = BsonSerializer.Deserialize<User>(userCollection.Find<BsonDocument>(userFilter).First());

            int expGained = new Random().Next(1, 4);
            var updateExp = Builders<BsonDocument>.Update.Set("Rank.CurrentExp", (user.Rank.CurrentExp + expGained));
            var updateTotalExp = Builders<BsonDocument>.Update.Set("Rank.TotalExp", (user.Rank.TotalExp + expGained));

            userCollection.UpdateOne(userFilter, updateExp);
            userCollection.UpdateOne(userFilter, updateTotalExp);
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
