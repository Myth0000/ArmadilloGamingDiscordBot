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
    public class StaffCommands : InteractionModuleBase<SocketInteractionContext>
    {
        [DefaultMemberPermissions(GuildPermission.Administrator)]
        [SlashCommand("say", "Send a message through the almighty Armadillo.")]
        public async Task HandleSay(string message)
        {
            await Context.Channel.SendMessageAsync(message);
            await RespondAsync("Message successfully delivered.", ephemeral:true);
        }
    }
}
