using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace ArmadilloGamingDiscordBot
{
    public class User
    {
        public ulong UserId { get; set; }
        public Rank Rank { get; set; } = new Rank();

        public User(ulong userId)
        {
            UserId = userId;
        }


        public bool ExistsInUserJson()
        {
            User[] users = JsonSerializer.Deserialize<User[]>(File.ReadAllText(DirectoryPaths.UsersJsonPath));

            foreach (User user in users)
            {
                if (user.UserId == UserId) { return true; }
            } return false;
        }
    }
}
