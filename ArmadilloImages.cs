using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmadilloGamingDiscordBot
{
    public static class ArmadilloImages
    {
        public static readonly string ArmadilloImagesPath = @"ArmadilloImages/";
        public static string RandomArmadilloImagePath()
        {
            string[] armadilloImagesArray = Directory.GetFiles(ArmadilloImagesPath);
            int randomNumber = new Random().Next(0, armadilloImagesArray.Length);

            return armadilloImagesArray[randomNumber];
        }
    }
}
