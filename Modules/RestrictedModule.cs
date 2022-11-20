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
    public class RestrictedModule : InteractionModuleBase<SocketInteractionContext>
    {
        MongoClient mongoClient = new MongoClient(Storage.MongoDBConnectionString);

        /*
        [SlashCommand("randomizeallitemid", "randomizes all virtual item ids into something else")]
        public async Task HandleRandomizeAllitemId()
        {
            await DeferAsync();
            var userCollection = mongoClient.GetDatabase("UserDatabase").GetCollection<BsonDocument>("User");
            await userCollection.Find(new BsonDocument()).ForEachAsync<BsonDocument>(document => {
                User user = BsonSerializer.Deserialize<User>(document);

                foreach(VirtualItem item in user.Inventory)
                {
                    item.Id = new Random().NextInt64(1000000000000, 9999999999999);
                }

                BsonDocument userBsonDoc = user.ToBsonDocument<User>();
                var userFilter = Builders<BsonDocument>.Filter.Eq("UserId", user.UserId);
                userCollection.ReplaceOne(userFilter, userBsonDoc);

            });
            await FollowupAsync("Item's Ids have been updated.");
        }


        
        [SlashCommand("quickfix", "Rolls in the new Virtual Items Update.")]
        public async Task HandleVirtualItemsUpdate()
        {
            var userCollection = mongoClient.GetDatabase("UserDatabase").GetCollection<BsonDocument>("User");
            List<User> userList = new();


            var userBsonDocumentList = userCollection.Find(new BsonDocument()).ToList();

            // everyone level 1 & higher are added to the userList
            foreach (var document in userBsonDocumentList)
            {
                User user = BsonSerializer.Deserialize<User>(document);
                if (user.Rank.Level >= 1) { userList.Add(user); }
            }


            // give lvl rewards based on their level
            foreach (User user in userList)
            {

                // give Virtual Items for people between levels 10/20
                if (user.Rank.Level > 10 && user.Rank.Level < 20)
                {
                    VirtualItem levelUpVirtualItem = VirtualItemSystem.GetRandomItemWithObtaining(mongoClient, "Level Up Rewards");
                    user.Inventory.Add(levelUpVirtualItem);
                    Context.Channel.SendMessageAsync($"{GuildEmotes.armadillo} {Context.Guild.GetUser(user.UserId).Mention} got a **{levelUpVirtualItem.Rarity}** {levelUpVirtualItem.EmoteId} for leveling up.");
                }

                // give Virtual Items for people between levels 10/20
                if (user.Rank.Level > 20)
                {
                    VirtualItem levelUpVirtualItem = VirtualItemSystem.GetRandomItemWithObtaining(mongoClient, "Level Up Rewards");
                    user.Inventory.Add(levelUpVirtualItem);
                    Context.Channel.SendMessageAsync($"{GuildEmotes.armadillo} {Context.Guild.GetUser(user.UserId).Mention} got a **{levelUpVirtualItem.Rarity}** {levelUpVirtualItem.EmoteId} for leveling up.");

                    VirtualItem levelUpVirtualItem1 = VirtualItemSystem.GetRandomItemWithObtaining(mongoClient, "Level Up Rewards");
                    user.Inventory.Add(levelUpVirtualItem1);
                    Context.Channel.SendMessageAsync($"{GuildEmotes.armadillo} {Context.Guild.GetUser(user.UserId).Mention} got a **{levelUpVirtualItem1.Rarity}** {levelUpVirtualItem1.EmoteId} for leveling up.");
                }


            }
            // update user database with new data
            foreach (User user in userList)
            {
                if(user.Inventory == null || user.Inventory.Count == 0)
                {
                    user.Inventory = new List<VirtualItem>();
                }
                var userFilter = Builders<BsonDocument>.Filter.Eq("UserId", user.UserId);

                var updateInventory = Builders<BsonDocument>.Update.Set("Inventory", user.Inventory);

                userCollection.UpdateOne(userFilter, updateInventory);
            }

            Context.Channel.SendMessageAsync("Successfully gave people their stuff.");
        }


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




           [SlashCommand("virtualttemsupdate", "Rolls in the new Virtual Items Update.")]
           public async Task HandleVirtualItemsUpdate()
           {
               var userCollection = mongoClient.GetDatabase("UserDatabase").GetCollection<BsonDocument>("User");
               List<User> userList = new();


               var userBsonDocumentList = userCollection.Find(new BsonDocument()).ToList();

               // everyone level 1 & higher are added to the userList
               foreach (var document in userBsonDocumentList)
               {
                   User user = BsonSerializer.Deserialize<User>(document);
                   if (user.Rank.Level >= 1) { userList.Add(user); }
               }


               // give lvl rewards based on their level
               foreach(User user in userList)
               {
                   user.Inventory = new List<VirtualItem>();

                   // update armadillo coins
                   user.ArmadilloCoin = CalculateArmadilloCoinsAtlevel(user.Rank.Level);

                   // give Virtual Items
                   if(user.Rank.Level % 10 == 0)
                   {
                       for (int i = user.Rank.Level / 10; i > 0; i--)
                       {
                           VirtualItem levelUpVirtualItem = VirtualItemSystem.GetRandomItemWithObtaining(mongoClient, "Level Up Rewards");
                           user.Inventory.Add(levelUpVirtualItem);
                           Context.Channel.SendMessageAsync($"{GuildEmotes.armadillo} {Context.Guild.GetUser(user.UserId).Mention} got a {levelUpVirtualItem.Rarity} {levelUpVirtualItem.EmoteId} for leveling up.");
                       }
                   }

                   // calculates how much armadillo coins a user has at a certain level
                   int CalculateArmadilloCoinsAtlevel(int atLevel)
                   {
                       int coin = 0;

                       for (int lvl = 1; lvl <= atLevel; lvl++)
                       {
                           coin += lvl + 5;
                           Console.WriteLine($"level : {lvl} | coins : {coin}");
                       }
                       return coin;
                   }
               }
               // update user database with new data
               foreach (User user in userList)
               {
                   var userFilter = Builders<BsonDocument>.Filter.Eq("UserId", user.UserId);

                   var updateArmadilloCoin = Builders<BsonDocument>.Update.Set("ArmadilloCoin", user.ArmadilloCoin);
                   var updateInventory = Builders<BsonDocument>.Update.Set("Inventory", user.Inventory);

                   userCollection.UpdateOne(userFilter, updateArmadilloCoin);
                   userCollection.UpdateOne(userFilter, updateInventory);
               }

               Context.Channel.SendMessageAsync("Successfully gave people their stuff.");
           }


   

        






        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////// C O M M E N T    T H E S E    O U T //////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        
         [SlashCommand("virtualitems", "Displays all the Virtual Items in the database")]
         public async Task HandleVirtualItems()
         {
             string virtualItems = "";
             string virtualItems1 = "";

             int itemCount = 0;

             foreach (VirtualItem item in VirtualItemSystem.GetAllItemsFromDatabase(mongoClient))
             {
                 itemCount++;
                 // discord only allows users to send up to 2000 characters in a message, so this is to avoid sending over 2000 characters in a single message.
                 if(itemCount >= 15)
                 {
                     virtualItems1 += $"{item.EmoteId} {item.Rarity} {item.Name} \n{item.Description}\n\n";
                     continue;
                 }
                 virtualItems += $"{item.EmoteId} {item.Rarity} {item.Name} \n{item.Description}\n\n";
             }

             await RespondAsync(virtualItems);
             if(virtualItems1 != "")
             {
                 Context.Channel.SendMessageAsync(virtualItems1);
             }
         }




         [SlashCommand("addvirtualitem", "Adds a Virtual Item to the database.")]
         public async Task HandleAddItem([Summary("emote", "A custom discord emoji to represent the Virtual Item.")] string emote, string imageUrl, [Summary("obtaining", "How is the Virtual Item obtained?")]
         [Choice("Level Up Rewards", "Level Up Rewards"), Choice("Classic VI Pack", "Classic VI Pack"), Choice("Deluxe VI Pack", "Deluxe VI Pack"), Choice("Unobtainable", "Unobtainable")]
         string obtaining,
         [Choice("COMMON", "COMMON"), Choice("RARE", "RARE"), Choice("UNIQUE", "UNIQUE"), Choice("LEGENDARY", "LEGENDARY"), Choice("MYTHIC", "MYTHIC")]
         string rarity, string description)
         {
             // emote is a string, it only becomes an actual emote when used in the discord chat
             VirtualItem item = VirtualItemSystem.ConvertEmoteIdToItem(emote, imageUrl, obtaining, rarity, description);

             VirtualItemSystem.AddItemToDatabase(mongoClient, item);
             await RespondAsync($"{item.EmoteId} {item.Name} | `{item.EmoteId}` | {item.Description} | {item.Obtaining} | {item.ImageUrl}");
         }




         [SlashCommand("grantitem", "Gives a Virtual Item to the user.")]
         public async Task HandleGiveVirtualItem([Summary("item", "Name of the Virtual Item")] string item, [Summary("user", "User to give the Virtual Item to.")] SocketUser user = null)
         {
            try
            {
                user ??= Context.User;
                VirtualItem virtualItem = VirtualItemSystem.GetItemFromDatabase(mongoClient, item);
                VirtualItemSystem.AddItemToUserInventory(mongoClient, virtualItem, user.Id);
                await RespondAsync($"{user.Mention} has been given {virtualItem.EmoteId}");
            } catch(Exception ex) { Console.WriteLine(ex); }
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




        [SlashCommand("deletetrades", "Deletes all the threads from discord & TradeDatabase.")]
        public async Task HandleDeleteAllThreads()
        {
            foreach(var channel in Context.Guild.ThreadChannels)
            {
                channel.DeleteAsync();
            }

            var tradeCollection = mongoClient.GetDatabase("TradeDatabase").GetCollection<BsonDocument>("Trade");
            var tradeBsonDocs = tradeCollection.Find<BsonDocument>(new BsonDocument()).ForEachAsync(document =>
            {
                tradeCollection.DeleteOne(document);
            });
        }




         [SlashCommand("copytestdatatomain", "Copies the data from the test database to the main database.")]
         public async Task HandleCopyDatabase([Choice("ArmadilloGaming", "ArmadilloGaming"), Choice("Test", "Test"), Summary("copyfrom", "The cluster you want to copy from.")]string copyfrom, string database, string collection)
         {

             if(copyfrom == "ArmadilloGaming")
             {
                 var mainDBclient = new MongoClient(Storage.ArmadillGamingDatabaseConnectionString);
                 var mainDBlist = mainDBclient.GetDatabase(database).GetCollection<BsonDocument>(collection).Find<BsonDocument>(new BsonDocument()).ToList();
                 var testDBcollection = mongoClient.GetDatabase(database).GetCollection<BsonDocument>(collection);

                 foreach (var item in mainDBlist)
                 {
                     testDBcollection.InsertOne(item);
                 }
                 Context.Channel.SendMessageAsync($"{collection} has been copied from {copyfrom} to the Test Database.");
                 return;
             }

             if (copyfrom == "Test")
             {
                 var mainDBclient = new MongoClient(Storage.ArmadillGamingDatabaseConnectionString);
                 var mainDBCollection = mainDBclient.GetDatabase(database).GetCollection<BsonDocument>(collection);
                 var testDBlist = mongoClient.GetDatabase(database).GetCollection<BsonDocument>(collection).Find<BsonDocument>(new BsonDocument()).ToList();

                 foreach (var item in testDBlist)
                 {
                     mainDBCollection.InsertOne(item);
                 }
                 Context.Channel.SendMessageAsync($"{collection} has been copied from {copyfrom} to the ArmadilloGaming Database.");
                 return;
             }

        
         }
 */




    }
}
