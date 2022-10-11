using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using Hypixel.NET;
using Hypixel.NET.GuildApi;

namespace ArmadilloGamingDiscordBot.Modules
{
    [EnabledInDm(false)]
    public class UtilitiesModule : InteractionModuleBase<SocketInteractionContext>
    {
        HypixelApi hypixel = new HypixelApi("d3d92668-0baa-4194-a006-1d2239e9be18", 300);

        [SlashCommand("armadillo", "Shows a cute picture of an Armadillo.")]
        public async Task HandleArmadillo()
        {
            await RespondWithFileAsync(ArmadilloImages.RandomArmadilloImagePath());
        }


        [SlashCommand("note", "Write down a private note that only you can see.")]
        public async Task HandleNote(string note, bool sendToDM=false)
        {
            await RespondAsync(note, ephemeral: true);
            if (sendToDM)
            {
                Context.User.SendMessageAsync(note);
            }
        }


        [SlashCommand("armadillo-rank", "Inspect the armadillo rank of a Armadillo follower.")]
        public async Task HandlePlayer(string player)
        {
            try
            {
                var armadilloGuild = hypixel.GetGuildByGuildName("armadillo gaming").Guild;
                var armadilloGuildMembers = armadilloGuild.Members;
                var minecraftPlayer = hypixel.GetUserByPlayerName(player).Player;

                if (MemberIsInGuild())
                {
                    var embed = new EmbedBuilder()
                        .WithAuthor(armadilloGuild.Name, iconUrl: Context.Guild.IconUrl)
                        .AddField(new EmbedFieldBuilder() { Name ="Armadillo Follower" , Value= $"`{minecraftPlayer.DisplayName}`" })
                        .AddField(new EmbedFieldBuilder() { Name = "Armadillo Rank", Value = $"`{GetGuildMemberRank()}`" })
                        .AddField(new EmbedFieldBuilder() { Name = "Armadillo Follower Since", Value=$"`{GetGuildMemberJoinedDateString()}`"})
                        .WithCurrentTimestamp();
                    await RespondAsync(embed: embed.Build());
                }
                else
                {
                    await RespondAsync("The player is not one with the Armadillo!", ephemeral: true);
                }

                // helper methods

                bool MemberIsInGuild()
                {
                    foreach (var member in armadilloGuildMembers)
                    {
                        if (member.Uuid == minecraftPlayer.Uuid)
                        {
                            return true;
                        }
                    }
                    return false;
                }


                string GetGuildMemberRank()
                {
                    foreach(var member in armadilloGuildMembers)
                    {
                        if(member.Uuid == minecraftPlayer.Uuid) { return member.Rank; }
                    }
                    return "n/a";
                }


                string GetGuildMemberJoinedDateString()
                {
                    foreach (var member in armadilloGuildMembers)
                    {
                        if (member.Uuid == minecraftPlayer.Uuid) { return member.Joined.ToString("dd MMMM yyyy hh:mm tt"); }
                    }
                    return "n/a";
                }
            }
            catch(Exception ex)
            {
                await RespondAsync("The player is not one with the Armadillo!", ephemeral: true);
            }
        }
    }
}
