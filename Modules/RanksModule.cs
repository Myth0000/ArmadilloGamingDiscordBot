using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord.WebSocket;
using Discord;

namespace ArmadilloGamingDiscordBot.Modules
{
    public class RanksModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("rank", "Displays the user's current rank.")]
        public async Task HandleRank()
        {

        }
    }
}
