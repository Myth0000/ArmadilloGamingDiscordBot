using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace ArmadilloGamingDiscordBot
{
    /// <summary>
    /// Contains useful methods to use when programming in Discord.Net
    /// </summary>
    public static class dnUtils
    {
        public static ButtonComponent GetButton(SocketMessageComponent messageComponent, string customId)
        {
            foreach (var i in (messageComponent.Message.Components))
            {
                if (i.GetType() == typeof(ActionRowComponent))
                {
                    foreach (var component in i.Components)
                    {
                        Console.WriteLine(component.GetType());
                        if (component.CustomId == customId) { return (ButtonComponent)component; }
                    }
                }
            }

            throw (new Exception($"ActionRowComponent inside SocketMessageComponent does not have the button with custom id {customId}"));
        }
    }
}
