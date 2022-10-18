using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Bson;


namespace ArmadilloGamingDiscordBot
{
    public class ClientEvents
    {

        MongoClient mongoClient = new MongoClient("mongodb+srv://Myth0000:JhgZ5shGWcxj3kEj@usercluster.djfruor.mongodb.net/?retryWrites=true&w=majority");

        public async Task MessageRecievedEvent(SocketMessage message)
        {

            if(message.Type == MessageType.ApplicationCommand) { return; } // if the message is used to run a slash command then return

            try
            {
                RankSystem.UpdateRankOnMessageSent(mongoClient, message);
            }
            catch (InvalidOperationException ex) // if user doesn't exist in UserDatabase 
            {
                Console.WriteLine("New user added to the database.");
                RankSystem.CreateNewUser(mongoClient, message.Author.Id);
            }
        }
    }
}
