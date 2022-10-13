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
                if(_currentExp != value)
                {
                    _currentExp = value;
                    CheckForLevelUp();
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


        // Increment/Decrememnt Methods of Properties \\
        public void IncreaseLevel(int amount = 1)
        {
            Level += amount;
            MaxExp = Convert.ToInt32(Math.Floor(MaxExp * 1.1));
        }

        public void DecreaseLevel(int amount = 1) => Level -= amount;

        public void IncreaseExp(int amount)
        {
            CurrentExp += amount;
            TotalExp += amount;
            
        }

        public void DecreaseExp(int amount) => _currentExp -= amount;

        // Resets \\
        public void ResetLevel() => Level = 0;
        public void ResetCurrentExp() => CurrentExp = 0;
        public void ResetMaxExp() => MaxExp = 0;
        public void ResetTotalExp() => TotalExp = 0;

        /// <summary>
        /// Resets all properties of this instance.
        /// </summary>
        public void ResetRank()
        {
            ResetLevel();
            ResetCurrentExp();
            ResetTotalExp();
            ResetMaxExp();
        }



        // Helper Methods
        public void CheckForLevelUp()
        {
            if (CurrentExp >= MaxExp)
            {
                CurrentExp -= MaxExp;
                IncreaseLevel();
            }
        }
    }
}
