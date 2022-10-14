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
        

        [SlashCommand("rank", "Displays the user's current rank.")]
        public async Task HandleRank()
        {
            List<User> users = JsonSerializer.Deserialize<List<User>>(File.ReadAllText(DirectoryPaths.UsersJsonPath));

            users.Add(new User(Context.User.Id));
            File.WriteAllText(DirectoryPaths.UsersJsonPath, JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true }));
            /*User[] users = JsonSerializer.Deserialize<User[]>(File.ReadAllText(DirectoryPaths.UsersJsonPath)); 
            foreach(User user in users)
            {
                await RespondAsync(user.UserId.ToString());
            }*/
        }
    }
}
