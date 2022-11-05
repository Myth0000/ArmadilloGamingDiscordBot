using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
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











        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////// C O M M E N T    T H E S E    O U T //////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        /*
        [SlashCommand("virtualitems", "Displays all the Virtual Items in the database")]
        public async Task HandleVirtualItems()
        {
            string virtualItems = "";

            foreach (VirtualItem item in VirtualItemSystem.GetAllItemsFromDatabase(mongoClient))
            {
                virtualItems += $"{item.EmoteId} {item.Rarity} {item.Name} \n{item.Description}\n\n";
            }

            await RespondAsync(virtualItems);
        }




        [SlashCommand("addvirtualitem", "Adds a Virtual Item to the database.")]
        public async Task HandleAddItem([Summary("emote", "A custom discord emoji to represent the Virtual Item.")] string emote, string imageUrl, [Summary("obtaining", "How is the Virtual Item obtained?")]
        [Choice("Level 10 Rewards", "Level 10 Rewards"), Choice("Level 20 Rewards", "Level 20 Rewards"), Choice("Level 30 Rewards", "Level 30 Rewards"),Choice("Unobtainable", "Unobtainable")]
        string obtaining,
        [Choice("COMMON", "COMMON"), Choice("RARE", "RARE"), Choice("UNIQUE", "UNIQUE"), Choice("LEGENDARY", "LEGENDARY"), Choice("MYTHIC", "MYTHIC")]
        string rarity, string description)
        {
            // emote is a string, it only becomes an actual emote when used in the discord chat
            VirtualItem item = VirtualItemSystem.ConvertEmoteIdToItem(emote, imageUrl, obtaining, rarity, description);

            VirtualItemSystem.AddItemToDatabase(mongoClient, item);
            await RespondAsync($"{item.EmoteId} {item.Name} | `{item.EmoteId}` | {item.Description} | {item.Obtaining} | {item.ImageUrl}");
        }




        [SlashCommand("givevirtualitem", "Gives a Virtual Item to the user.")]
        public async Task HandleGiveVirtualItem([Summary("item", "Name of the Virtual Item")] string item, [Summary("user", "User to give the Virtual Item to.")] SocketUser user = null)
        {
            user ??= Context.User;
            VirtualItem virtualItem = VirtualItemSystem.GetItemFromDatabase(mongoClient, item);
            VirtualItemSystem.AddItemToUserInventory(mongoClient, virtualItem, user.Id);
            await RespondAsync($"{user.Mention} has been given {virtualItem.EmoteId}");
        }




        [SlashCommand("addshopitem", "Adds an item to the shop for purchase.")]
        public async Task HandleAddShopItem(string item_name, string description, int price, [Choice("Virtual Item", "Virtual Item")] string itemType)
        {
            ShopItem shopItem = new()
            {
                Name = item_name,
                Description = description,
                Price = price,
                ItemType = itemType
            };

            ShopSystem.AddItemToShop(mongoClient, shopItem);

            await RespondAsync("The item has been added to the shop.", ephemeral: true);
        }
        */




    }
}
