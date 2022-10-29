using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace ArmadilloGamingDiscordBot.Modules
{
    public class ItemsModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("inventory", "Displays a list of items the user owns.")]
        public async Task HandleInventory()
        {

            string userInventory = GuildEmotes.First_Item;

            var inventoryEmbed = new EmbedBuilder()
                .WithAuthor($"{Context.User.Username}'s Inventory", iconUrl: Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl())
                .AddField(new EmbedFieldBuilder() { Name = "Items", Value = userInventory })
                .WithCurrentTimestamp()
                .Build();

            await RespondAsync(embed:inventoryEmbed);
        }


    }
}
