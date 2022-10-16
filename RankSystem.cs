using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace ArmadilloGamingDiscordBot
{
    public static class RankSystem
    {
        public static void displayRankProperties(Rank rank)
        {
            Console.WriteLine($"Level: {rank.Level}\nExp: {rank.CurrentExp}/{rank.MaxExp}\nTotal Exp: {rank.TotalExp}");
        }




        /// <summary>
        /// Updates the rank to make sure level ups, etc. are happening.
        /// </summary>
        public static Rank UpdateRankOnMessageSent(Rank rank, SocketMessage message)
        {
            // updates currentExp & totalExp
            int expEarned = new Random().Next(10, 50);
            rank.CurrentExp += expEarned;
            rank.TotalExp += expEarned;

            // Level up
            if (rank.CurrentExp >= rank.MaxExp)
            {
                int count = Convert.ToInt32(Math.Floor(Convert.ToDecimal(rank.CurrentExp / rank.MaxExp)));
                Console.WriteLine(count);
                while(count-- > 0)
                {
                    Console.WriteLine("in while loop");
                    rank.Level++;
                    rank.CurrentExp -= rank.MaxExp;
                    Console.WriteLine(rank.CurrentExp);
                    rank.MaxExp = Convert.ToInt32(Math.Floor(1.1 * rank.MaxExp));
                    message.Channel.SendMessageAsync($"{message.Author.Mention} has leveled up to level {rank.Level}!");
                }
            }
            displayRankProperties(rank);
            return rank;
        }



    }




    public class Rank
    {
        /// <summary>
        /// The current level.
        /// </summary>
        public int Level { get; set; } = 0;

        /// <summary>
        /// The amount of Exp in possession.
        /// </summary>
        public int CurrentExp { get; set; } = 0;

        /// <summary>
        /// The maximum amount of Exp needed to level up to the next level.
        /// </summary>
        public int MaxExp { get; set; } = 10;

        /// <summary>
        /// The total amount of Exp in possession since level 0.
        /// </summary>
        public int TotalExp { get; set; } = 0;
    }
}
