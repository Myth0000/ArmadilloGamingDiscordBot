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
    public class ShopModule : InteractionModuleBase<SocketInteractionContext>
    {
        MongoClient mongoClient = new(Storage.MongoDBConnectionString);

        [SlashCommand("shop", "A place to shop Virtual Items.")]
        public async Task HandleShop()
        {
            string shopItemsDisplay = "";

            foreach(var item in ShopSystem.GetAllItemsFromShop(mongoClient))
            {
                shopItemsDisplay += $"**{item.Name}** {GuildEmotes.armadilloCoin}{item.Price}\n{GuildEmotes.armadillo}{item.Description}\n";
            }
            if(shopItemsDisplay.Length == 0) { shopItemsDisplay = "There are no items available for purchase."; }

            Embed shopEmbed = new EmbedBuilder()
                .WithAuthor("Shop", iconUrl: Context.Guild.IconUrl)
                .AddField(new EmbedFieldBuilder() { Name = "Virtual Items", Value = shopItemsDisplay })
                .WithCurrentTimestamp()
                .Build();

            var selectMenu = new SelectMenuBuilder()
                .WithCustomId("ShopSelectMenu")
                .WithMinValues(1)
                .WithMaxValues(1)
                .WithPlaceholder("Select an item to purchase.")
                .AddOption("Random Virtual Item", "Random Virtual Item", description: "Gives you a random Virtual Item.");

            ComponentBuilder component = new ComponentBuilder()
                .WithSelectMenu(selectMenu);

            await RespondAsync(embed: shopEmbed, components: component.Build());
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
