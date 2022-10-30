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

                foreach (VirtualItem item in VirtualItemSystem.GetUserInventory(mongoClient, user.Id))
                {
                    userInventory += item.EmoteId;
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
                    .AddField(new EmbedFieldBuilder() { Name = "Items", Value = items })
                    .WithCurrentTimestamp()
                    .Build();
            }
        }


        [SlashCommand("previewitem", "Allows the user to preview a Virtual Item.")]
        public async Task HandlePreviewItem(string Item)
        {

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
    }
}
