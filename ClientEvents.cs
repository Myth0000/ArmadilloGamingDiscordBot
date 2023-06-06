using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;


namespace ArmadilloGamingDiscordBot
{
    public class ClientEvents
    {

        MongoClient mongoClient = new(Storage.MongoDBConnectionString);

        public async Task MessageRecievedEvent(SocketMessage message)
        {

            // If the message is a slash command OR the message was in DMs OR the message was not from armadillogaming/discordbottest servers then return.
            ulong guildId = (message.Channel as SocketGuildChannel).Guild.Id;
            if ((message.Type == MessageType.ApplicationCommand) || (message.Channel.ToString().First<char>() == '@') ||
                ((guildId != Storage.DiscordBotTestGuildId) && (guildId != Storage.ArmadilloGamingGuildId))) { return; }

            try
            {
                var userCollection = mongoClient.GetDatabase("UserDatabase").GetCollection<BsonDocument>("User");
                User user = BsonSerializer.Deserialize<User>(userCollection.Find<BsonDocument>(Builders<BsonDocument>.Filter.Eq("UserId", message.Author.Id)).First());

                if (RankSystem.ExpGainIsOnCooldown(mongoClient, user)) { return; }

                RankSystem.UpdateRankOnMessageSent(mongoClient, message);
            }
            catch (InvalidOperationException ex) // if user doesn't exist in UserDatabase 
            {
                Console.WriteLine("New user added to the database.");
                RankSystem.CreateNewUser(mongoClient, message.Author.Id);
            }
        }


        public async Task UserLeftGuild(SocketGuild guild, SocketUser user)
        {
            // Deletes users who leave their guild from the database
            var userCollection = mongoClient.GetDatabase("UserDatabase").GetCollection<BsonDocument>("User");
            var findUserFilter = Builders<BsonDocument>.Filter.Eq("UserId", user.Id);

            userCollection.DeleteOne(findUserFilter);
        }
    }
}
