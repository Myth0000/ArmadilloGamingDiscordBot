using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Text.Json;


namespace ArmadilloGamingDiscordBot
{
    public static class ClientEvents
    {
        public static async Task MessageRecievedEvent(SocketMessage message)
        {
            try
            {
                List<User> users = JsonSerializer.Deserialize<List<User>>(File.ReadAllText(DirectoryPaths.UsersJsonPath));

               if (users.Count == 0)
                {
                    users.Add(new User(message.Author.Id));
                }
                else
                {
                    foreach (User user in users)
                    {
                        if (user.ExistsInUserJson())
                        {
                            user.Rank.CurrentExp = 3 + new Random().Next(0, 100);
                        }
                    }
                    
                }
                    
               File.WriteAllText(DirectoryPaths.UsersJsonPath, JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch(Exception ex) { Console.WriteLine(ex.ToString()); }
        }
    }
}
