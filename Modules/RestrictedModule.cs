using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord.WebSocket;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace ArmadilloGamingDiscordBot.Modules
{
    /// <summary>
    /// These commands are exclusive to the owner of the discord bot since they are mostly used to edit the database, etc.
    /// </summary>
    [RequireOwner]
    [DefaultMemberPermissions(Discord.GuildPermission.Administrator)]
    public class RestrictedModule : InteractionModuleBase<SocketInteractionContext>
    {
        MongoClient mongoClient = new MongoClient(Storage.TestBotDatabaseConnectionString);


        // create a command that will check if a user doesn't have a certain property in User class, if they don't have it then add it to their data
        [SlashCommand("updateuserdata", "DANGER! Developer only command. Updates UserDatabase to keep it up to date.")]
        public async Task HandleUpdateUserData()
        {
            var userCollection = mongoClient.GetDatabase("UserDatabase").GetCollection<BsonDocument>("User");
            await userCollection.Find(new BsonDocument()).ForEachAsync<BsonDocument>(document =>{
                User user = BsonSerializer.Deserialize<User>(document);
                BsonDocument userBsonDoc = user.ToBsonDocument<User>();
                var userFilter = Builders<BsonDocument>.Filter.Eq("UserId", user.UserId);
                userCollection.ReplaceOne(userFilter, userBsonDoc);

            });


            await RespondAsync("All User data in the database match User class properties.", ephemeral:true);
        }
    }
}
