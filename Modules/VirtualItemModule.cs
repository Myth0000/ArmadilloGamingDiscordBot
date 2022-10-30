using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MongoDB.Driver;
using ArmadilloGamingDiscordBot.Blueprints;
using ArmadilloGamingDiscordBot;


namespace ArmadilloGamingDiscordBot.Modules
{
    public class VirtualItemModule : InteractionModuleBase<SocketInteractionContext>
    {
        MongoClient mongoClient = new(Storage.TestBotDatabaseConnectionString);

        [SlashCommand("inventory", "Displays a list of Virtual Items the user owns.")]
        public async Task HandleInventory(SocketUser user = null)
        {
            user ??= Context.User;
            try
            {
                string userInventory = "";

                int count = 0;
                foreach (VirtualItem item in VirtualItemSystem.GetUserInventory(mongoClient, user.Id))
                {
                    userInventory += $" {item.EmoteId}";
                    count++;

                    if(count % 5 == 0) { userInventory += "\n"; } // adds a new line to make the inv look nicer every 5 items
                }

                await RespondAsync(embed: BuildInventoryEmbed(userInventory));
            } catch(NullReferenceException nullException)
            {
                await RespondAsync(embed: BuildInventoryEmbed("This user does not have any items."));    
            }


            Embed BuildInventoryEmbed(string items)
            {
                return new EmbedBuilder()
                    .WithAuthor($"{user.Username}'s Inventory", iconUrl: user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                    .AddField(new EmbedFieldBuilder() { Name = "Virtual Items", Value = items })
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
                    .AddField(new EmbedFieldBuilder() { Name = $"{virtualItem.EmoteId} {virtualItem.Name}", Value = virtualItem.Description })
                    .WithCurrentTimestamp()
                    .Build();

                await RespondAsync(embed: itemPreviewEmbed);
            } catch(Exception ex) { await RespondAsync("That item does not exist, please make sure you spelled it correctly. Virtual Item names are case-sensitive.", ephemeral:true); }
        }


        [DefaultMemberPermissions(GuildPermission.Administrator)]
        [SlashCommand("addvirtualitem", "Adds a Virtual Item to the database.")]
        public async Task HandleAddItem([Summary("emote", "A custom discord emoji to represent the Virtual Item.")] string emote, string description)
        {
            // emote is a string, it only becomes an actual emote when used in the discord chat
            VirtualItem item = VirtualItemSystem.ConvertEmoteIdToItem(emote, description);

            VirtualItemSystem.AddItemToDatabase(mongoClient, item);
            await RespondAsync($"{item.EmoteId} {item.Name} | `{item.EmoteId}` | {item.Description}");
        }


        [DefaultMemberPermissions(GuildPermission.Administrator)]
        [SlashCommand("givevirtualitem", "Gives a Virtual Item to the user.")]
        public async Task HandleGiveVirtualItem([Summary("item", "Name of the Virtual Item")]string item, [Summary("user", "User to give the Virtual Item to.")] SocketUser user = null)
        {
            user ??= Context.User;
            VirtualItem virtualItem = VirtualItemSystem.GetItemFromDatabase(mongoClient, item);
            VirtualItemSystem.AddItemToUserInventory(mongoClient, virtualItem, user.Id);
            await RespondAsync($"{user.Mention} has been given {virtualItem.EmoteId}");
        }
    }
}
