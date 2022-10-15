using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmadilloGamingDiscordBot
{
    /// <summary>
    /// Holds the core properties of a level system.
    /// </summary>
    public class Rank
    {
        private int _level = 0;
        private int _currentExp = 0;
        private int _maxExp = 10;
        private int _totalExp = 0;
        private static int setterInvokedInCurrentExpCount = 0; // keeps count of how many times setter invoked ( setter in properties invokes 3 times for some reason?? )
        /// <summary>
        /// The current level.
        /// </summary>
        public int Level
        {
            get { return _level; }
            set { _level = value; }
        } 
        
        /// <summary>
        /// The amount of Exp in possession.
        /// </summary>
        public int CurrentExp
        {
            get { return _currentExp; }
            set
            {
                _currentExp = value;
                setterInvokedInCurrentExpCount++;
                Console.WriteLine(setterInvokedInCurrentExpCount);
                if(CurrentExp >= MaxExp && setterInvokedInCurrentExpCount == 3)
                {
                    setterInvokedInCurrentExpCount = 0;
                    Console.WriteLine($"Ready to lvl up! {CurrentExp}/{MaxExp}");
                }
            }
        }

        /// <summary>
        /// The maximum amount of Exp needed to level up to the next level.
        /// </summary>
        public int MaxExp
        {
            get { return _maxExp; }
            set { _maxExp = value; }
        }

        /// <summary>
        /// The total amount of Exp in possession since level 0.
        /// </summary>
        public int TotalExp
        {
            get { return _totalExp; }
            set { _totalExp = value; }
        }


        /// <summary>
        /// Resets all properties of this instance.
        /// </summary>
        public void ResetRank()
        {
            Level = 0;
            CurrentExp = 0;
            TotalExp = 0;
            MaxExp = 10;
        }
    }
}
