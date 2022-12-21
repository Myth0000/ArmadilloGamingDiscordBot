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
            await DeferAsync(ephemeral: true);
            await FollowupAsync(note, ephemeral: true);
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




        [SlashCommand("help", "Helps inform you about how the bot functions.")]
        public async Task HandleHelp([Choice("Slash Commands", "Slash Commands"), Choice("Levels", "Levels"), Choice("Shop", "Shop"), Choice("Virtual Items", "Virtual Items")] string topic = null)
        {
            EmbedBuilder helpEmbed = new EmbedBuilder();
            helpEmbed.WithAuthor($"{topic} Help", iconUrl:Context.Guild.IconUrl);

            switch (topic)
            {
                case "Slash Commands":

                    var listOfSlashCommands = await Context.Guild.GetApplicationCommandsAsync();
                    string publicSlashCommands = "";
                    string adminSlashCommands = "";

                    foreach (var command in listOfSlashCommands)
                    {
                        if((command.Type == ApplicationCommandType.Slash))
                        {
                            if ((!command.DefaultMemberPermissions.Administrator))
                            {
                                publicSlashCommands += $"{GuildEmotes.armadillo}/{command.Name}\n";
                            }
                            else // if a admin runs this command they'll also be able to see admin only commands
                            {
                                adminSlashCommands += $"{GuildEmotes.armadillo}/{command.Name}\n";
                            }
                        }
                    }
                    
                    if((Context.User as SocketGuildUser).GuildPermissions.Administrator)
                    {
                        helpEmbed.WithDescription($"These are the list of available slash commands\n**Public Commands**\n{publicSlashCommands}\n**Admin Commands**\n{adminSlashCommands}");
                    }
                    else
                    {
                        helpEmbed.WithDescription($"**These are the list of available public slash commands**\n{publicSlashCommands}");
                    }

                    break;
                case "Levels":
                    helpEmbed.WithDescription
                        (@$"
**What are Levels?**
{GuildEmotes.armadillo} Levels are a way to measure how active you are in the server.
                            
**How do I level up?**
{GuildEmotes.armadillo} Sending messages in text channels

**Exp**
{GuildEmotes.armadillo} To check the exp gain cooldown, use the slash command **`/settings`**

**What are the benefits of leveling up?**
{GuildEmotes.armadillo} You gain {Storage.ArmadilloCoinEmoteId}
{GuildEmotes.armadillo} You gain a Virtual Item every 10 levels
{GuildEmotes.armadillo} You gain a pat on the back!

**Slash Commands associated  with levels**
{GuildEmotes.armadillo} /level
{GuildEmotes.armadillo} /leaderboard
                        ");
                    break;
                case "Shop":
                    helpEmbed.WithDescription
                        (@$"
**What is the shop?**
{GuildEmotes.armadillo} The shop is a place to buy Virtual Items.

**Shop Currency**
{GuildEmotes.armadillo} {Storage.ArmadilloCoinEmoteId} Armadillo Coins

**How do you obtain {Storage.ArmadilloCoinEmoteId}**
{GuildEmotes.armadillo} Leveling up gives {Storage.ArmadilloCoinEmoteId}

**Slash Commands associated with the shop**
{GuildEmotes.armadillo} /shop
{GuildEmotes.armadillo} /inventory
                        ");
                    break;
                case "Virtual Items":
                    helpEmbed.WithDescription
                        (@$"
**What is a Virtual Item?**
{GuildEmotes.armadillo} Virtual Items are collectibles.

**How do you obtain Virtual Items?**
{GuildEmotes.armadillo} You gain a Virtual Item every 10 levels
{GuildEmotes.armadillo} You can buy Virtual Items from the shop
{GuildEmotes.armadillo} You can trade Virtual Items with other players

**Slash Commands associated with Virtual Items**
{GuildEmotes.armadillo} /trade
{GuildEmotes.armadillo} /previewitem
{GuildEmotes.armadillo} /inventory
                        ");
                    break;
                default:
                    helpEmbed
                        .WithAuthor("Help", iconUrl: Context.Guild.IconUrl)
                        .WithDescription("To get help on a specific topic, please type **`/help topic`**");
                    break;
            }

            await RespondAsync(embed: helpEmbed.Build());
        }
    }
}
