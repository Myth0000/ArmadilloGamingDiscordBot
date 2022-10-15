using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord.WebSocket;
using Discord;
using System.Text.Json;

namespace ArmadilloGamingDiscordBot.Modules
{
    public class RanksModule : InteractionModuleBase<SocketInteractionContext>
    {
        

        [SlashCommand("level", "Displays the user's current rank.")]
        public async Task HandleRank()
        {
            List<User> users = JsonSerializer.Deserialize<List<User>>(File.ReadAllText(DirectoryPaths.UsersJsonPath));

            var embedMsg = ReturnEmbedMessage();
            if(embedMsg != null) { await RespondAsync(embed: embedMsg); return; }

            // if user is not found in the Users.Json file then add them to it
            users.Add(new User(Context.User.Id));
            File.WriteAllText(DirectoryPaths.UsersJsonPath, JsonSerializer.Serialize(users, new JsonSerializerOptions() { WriteIndented=true}));

            await RespondAsync(embed: ReturnEmbedMessage());

            Embed ReturnEmbedMessage()
            {
                foreach (User user in users)
                {
                    if (user.UserId == Context.User.Id)
                    {

                        Embed rankEmbed = new EmbedBuilder()
                            .WithAuthor($"{Context.User.Username}#{Context.User.Discriminator}", iconUrl: Context.User.GetAvatarUrl())
                            .AddField(new EmbedFieldBuilder() { Name = $"Level {user.Rank.Level}", Value = $"{user.Rank.CurrentExp}/{user.Rank.MaxExp}" })
                            .AddField(new EmbedFieldBuilder() { Name = "Total Exp", Value = user.Rank.TotalExp })
                            .WithCurrentTimestamp()
                            .Build();

                        return rankEmbed;
                    }
                }
                return null;
            }
        }
    }
}
