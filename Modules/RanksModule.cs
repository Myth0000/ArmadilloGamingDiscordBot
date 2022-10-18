using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord.WebSocket;
using Discord;
using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace ArmadilloGamingDiscordBot.Modules
{
    public class RanksModule : InteractionModuleBase<SocketInteractionContext>
    {
        private MongoClient mongoClient = new("mongodb+srv://Myth0000:JhgZ5shGWcxj3kEj@usercluster.djfruor.mongodb.net/?retryWrites=true&w=majority");

        [SlashCommand("level", "Displays the user's current rank.")]
        public async Task HandleRank()
        {
            try
            {
                await RespondAsync(embed: BuildRankEmbed());
            }
            catch (InvalidOperationException ex) // user doesn't exist in database
            {
                RankSystem.CreateNewUser(mongoClient, Context.User.Id);
                await RespondAsync(embed: BuildRankEmbed());
            }

            Embed BuildRankEmbed()
            {
                var userCollection = mongoClient.GetDatabase("UserDatabase").GetCollection<BsonDocument>("User");
                var userFilter = Builders<BsonDocument>.Filter.Eq("UserId", Context.User.Id);

                Rank userRank = BsonSerializer.Deserialize<User>(userCollection.Find<BsonDocument>(userFilter).First()).Rank;

                return new EmbedBuilder()
                    .WithAuthor($"{Context.User.Username}#{Context.User.Discriminator}", iconUrl: Context.User.GetAvatarUrl())
                    .AddField(new EmbedFieldBuilder() { Name = $"Level {userRank.Level}", Value = $"{userRank.CurrentExp}/{userRank.MaxExp}" })
                    .AddField(new EmbedFieldBuilder() { Name = "Total Exp", Value = userRank.TotalExp })
                    .WithCurrentTimestamp()
                    .Build();
            }
        }
    }
}
