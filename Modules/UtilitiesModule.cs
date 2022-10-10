using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Interactions;

namespace ArmadilloGamingDiscordBot.Modules
{
    [EnabledInDm(false)]
    public class UtilitiesModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("armadillo", "Shows a cute picture of an Armadillo.")]
        public async Task HandleArmadillo()
        {
            await RespondWithFileAsync(ArmadilloImages.RandomArmadilloImagePath());
        }
    }
}
