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
using ArmadilloGamingDiscordBot;


namespace ArmadilloGamingDiscordBot.Modules
{
    public class VirtualItemModule : InteractionModuleBase<SocketInteractionContext>
    {
        MongoClient mongoClient = new(Storage.MongoDBConnectionString);

        [SlashCommand("inventory", "Displays a list of Virtual Items the user owns.")]
        public async Task HandleInventory(SocketUser user = null)
        {
            user ??= Context.User;
            try
            {
                string commonVirtualItems = "";
                string rareVirtualItems = "";
                string uniqueVirtualItems = "";
                string legendaryVirtualItems = "";
                string mythicVirtualItems = "";

                foreach (VirtualItem item in VirtualItemSystem.GetUserInventory(mongoClient, user.Id))
                {
                    // organizes item in inventory into categories
                    switch (item.Rarity)
                    {
                        case "COMMON":
                            commonVirtualItems += $" {item.EmoteId}";
                            break;
                        case "RARE":
                            rareVirtualItems += $" {item.EmoteId}";
                            break;
                        case "UNIQUE":
                            uniqueVirtualItems += $" {item.EmoteId}";
                            break;
                        case "LEGENDARY":
                            legendaryVirtualItems += $" {item.EmoteId}";
                            break;
                        case "MYTHIC":
                            mythicVirtualItems += $" {item.EmoteId}";
                            break;
                    }
                }

                // If user doesn't have a item for one of the categories, it'll display "None"
                if (commonVirtualItems.Length == 0) { commonVirtualItems = "None"; }
                if (rareVirtualItems.Length == 0) { rareVirtualItems = "None"; }
                if (uniqueVirtualItems.Length == 0) { uniqueVirtualItems = "None"; }
                if (legendaryVirtualItems.Length == 0) { legendaryVirtualItems = "None"; }
                if (mythicVirtualItems.Length == 0) { mythicVirtualItems = "None"; }

                await RespondAsync(embed: BuildInventoryEmbed(commonVirtualItems, rareVirtualItems, uniqueVirtualItems, legendaryVirtualItems, mythicVirtualItems));
            } catch(NullReferenceException nullException)
            {
                string item = "None";
                await RespondAsync(embed: BuildInventoryEmbed(item, item, item, item, item));    
            } catch (InvalidOperationException ex) // user doesn't exist in database
            {
                RankSystem.CreateNewUser(mongoClient, user.Id);
                string item = "None";
                await RespondAsync(embed: BuildInventoryEmbed(item, item, item, item, item));
            }

            Embed BuildInventoryEmbed(string commonItem, string rareItem, string uniqueItem, string legendaryItem, string mythicItem)
            {
                var userCollection = mongoClient.GetDatabase("UserDatabase").GetCollection<BsonDocument>("User");
                var userFilter = Builders<BsonDocument>.Filter.Eq("UserId", user.Id);
                User _user = BsonSerializer.Deserialize<User>(userCollection.Find<BsonDocument>(userFilter).First());

                return new EmbedBuilder()
                    .WithAuthor($"{user.Username}'s Inventory", iconUrl: user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                    .AddField(new EmbedFieldBuilder() { Name = "Armadillo Coins", Value = $"{Storage.ArmadilloCoinEmoteId} {_user.ArmadilloCoin}\n\n**Virtual Items**" })
                    .AddField(new EmbedFieldBuilder() { Name = "COMMON", Value = commonItem})
                    .AddField(new EmbedFieldBuilder() { Name = "RARE", Value = rareItem})
                    .AddField(new EmbedFieldBuilder() { Name = "UNIQUE", Value = uniqueItem})
                    .AddField(new EmbedFieldBuilder() { Name = "LEGENDARY", Value = legendaryItem})
                    .AddField(new EmbedFieldBuilder() { Name = "MYTHIC", Value = mythicItem})
                    .WithCurrentTimestamp()
                    .Build();
            }
        }




        [SlashCommand("previewitem", "Allows the user to preview a Virtual Item.")]
        public async Task HandlePreviewItem([Summary("item" ,"Name of the Virtual Item.")]string item)
        {
            try
            {
                VirtualItem virtualItem = VirtualItemSystem.GetItemFromDatabase(mongoClient, item);
                Embed itemPreviewEmbed = new EmbedBuilder()
                    .AddField(new EmbedFieldBuilder() { Name=$"{virtualItem.EmoteId} {virtualItem.Rarity} Virtual Item", Value="** **"})
                    .AddField(new EmbedFieldBuilder() { Name = "Name", Value = virtualItem.Name })
                    .AddField(new EmbedFieldBuilder() { Name="Description", Value= virtualItem.Description})
                    .AddField(new EmbedFieldBuilder() { Name="Obtainable Through", Value=virtualItem.Obtaining})
                    .WithThumbnailUrl(virtualItem.ImageUrl)
                    .WithCurrentTimestamp()
                    .Build();

                await RespondAsync(embed: itemPreviewEmbed);
            } catch(Exception ex) { await RespondAsync("That item does not exist, please make sure you spelled it correctly. Virtual Item names are case-sensitive.", ephemeral:true); }
        }




    }
}
