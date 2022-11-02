﻿using System;
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
    public class ShopModule : InteractionModuleBase<SocketInteractionContext>
    {
        MongoClient mongoClient = new(Storage.MongoDBConnectionString);

        [SlashCommand("shop", "A place to shop Virtual Items.")]
        public async Task HandleShop()
        {
            string shopItemsDisplay = "";

            var selectMenu = new SelectMenuBuilder()
                .WithCustomId("ShopSelectMenu")
                .WithMinValues(1)
                .WithMaxValues(1)
                .WithPlaceholder("Select an item to purchase.");

            // creates the shop display text the user will see with all the shop items & their prices/descriptions
            foreach (var item in ShopSystem.GetAllItemsFromShop(mongoClient))
            {
                shopItemsDisplay += $"**{item.Name}** {GuildEmotes.armadilloCoin}{item.Price}\n{GuildEmotes.armadillo}{item.Description}\n";
                selectMenu.AddOption(item.Name, item.Name, item.Description);
            }
            if(shopItemsDisplay.Length == 0) { shopItemsDisplay = "There are no items available for purchase."; }

            Embed shopEmbed = new EmbedBuilder()
                .WithAuthor("Shop", iconUrl: Context.Guild.IconUrl)
                .AddField(new EmbedFieldBuilder() { Name = "Virtual Items", Value = shopItemsDisplay })
                .WithCurrentTimestamp()
                .Build();

            ComponentBuilder component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            if(selectMenu.Options.Count == 0) { await RespondAsync(embed: shopEmbed); return; } // if there are no items to select in the menu run this

            await RespondAsync(embed: shopEmbed, components: component.Build());
        }




        [ComponentInteraction("ShopSelectMenu")]
        public async Task HandleShopSelectMenu(string[] inputs)
        {
            // depending on what the selected item is, 
            foreach (var item in ShopSystem.GetAllItemsFromShop(mongoClient))
            {
                if ((inputs[0] == item.Name) && (item.ItemType == "Virtual Item"))
                {
                    var userCollection = mongoClient.GetDatabase("UserDatabase").GetCollection<BsonDocument>("User");
                    var userFilter = Builders<BsonDocument>.Filter.Eq("UserId", Context.User.Id);
                    User user = BsonSerializer.Deserialize<User>(userCollection.Find<BsonDocument>(userFilter).First());

                    // if user can afford the item
                    if(item.Price <= user.ArmadilloCoin)
                    {
                        VirtualItem virtualItem = VirtualItemSystem.GetRandomItemWithObtaining(mongoClient, "Classic VI Pack");

                        user.ArmadilloCoin -= item.Price;
                        user.Inventory.Add(virtualItem);

                        var updateArmadilloCoins = Builders<BsonDocument>.Update.Set("ArmadilloCoin", user.ArmadilloCoin);
                        var updateInventory = Builders<BsonDocument>.Update.Set("Inventory", user.Inventory);

                        userCollection.UpdateOne(userFilter, updateArmadilloCoins);
                        userCollection.UpdateOne(userFilter, updateInventory);

                        await RespondAsync($"{Context.User.Mention} found a **{virtualItem.Rarity} {virtualItem.Name}** inside the {item.Name}.");
                    }
                }
            }

        }




        [DefaultMemberPermissions(GuildPermission.Administrator)]
        [SlashCommand("addshopitem", "Adds an item to the shop for purchase.")]
        public async Task HandleAddShopItem(string item_name, string description, int price, [Choice("Virtual Item", "Virtual Item")]string itemType)
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
    }
}
