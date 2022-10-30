﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord.WebSocket;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using ArmadilloGamingDiscordBot.Blueprints;

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


        [SlashCommand("updatevirtualitemdata", "DANGER! Developer only command. Updates VirtualItemDatabase to keep it up to date.")]
        public async Task HandleVirtualItemData()
        {
            var virtualItemCollection = mongoClient.GetDatabase("VirtualItemDatabase").GetCollection<BsonDocument>("VirtualItem");
            await virtualItemCollection.Find(new BsonDocument()).ForEachAsync<BsonDocument>(document => {
                VirtualItem virtualItem = BsonSerializer.Deserialize<VirtualItem>(document);
                BsonDocument virtualItemBsonDoc = virtualItem.ToBsonDocument<VirtualItem>();

                var virtualItemFilter = Builders<BsonDocument>.Filter.Eq("Name", virtualItem.Name);
                virtualItemCollection.ReplaceOne(virtualItemFilter, virtualItemBsonDoc);

            });


            await RespondAsync("All VirtualItem data in the database match VirtualItem class properties.", ephemeral: true);
        }





    }
}
