using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using ArmadilloGamingDiscordBot.Blueprints;

namespace ArmadilloGamingDiscordBot
{
    public class TradeSystem
    {
        public static ComponentBuilder TradeRequestComponent(SocketInteractionContext context, SocketGuildUser tradeRequestSentTo, bool disableButtons = false)
        {
            ButtonBuilder acceptTradeRequestButton = new ButtonBuilder()
                .WithCustomId($"traderequest_accept:{context.User.Id},{tradeRequestSentTo.Id}")
                .WithLabel("Accept")
                .WithStyle(ButtonStyle.Success);

            ButtonBuilder declineTradeRequestButton = new ButtonBuilder()
                .WithCustomId($"traderequest_decline:{context.User.Id},{tradeRequestSentTo.Id}")
                .WithLabel("Decline")
                .WithStyle(ButtonStyle.Danger);

            if (disableButtons)
            {
                acceptTradeRequestButton.IsDisabled = true;
                declineTradeRequestButton.IsDisabled = true;
            }

            ComponentBuilder tradeRequestComponents = new ComponentBuilder()
                .WithButton(acceptTradeRequestButton)
                .WithButton(declineTradeRequestButton);

            return tradeRequestComponents;
        }




        public static ComponentBuilder TradeMenuComponents(MongoClient mongoClient, SocketGuildUser user, string selectMenuCustomId)
        {
            SelectMenuBuilder tradeItemsSelectMenu = new SelectMenuBuilder()
                .WithCustomId(selectMenuCustomId)
                .WithMaxValues(1)
                .WithMinValues(0)
                .WithPlaceholder("Select a Virtual Item to trade");

            try
            {
                AddVirtualItemsToSelectMenuOptions();
            }
            catch(InvalidOperationException ex) // happens when user is not in the database
            {
                RankSystem.CreateNewUser(mongoClient, user.Id);
                AddVirtualItemsToSelectMenuOptions();
            }



            ComponentBuilder components = new ComponentBuilder()
                .WithSelectMenu(tradeItemsSelectMenu);

            void AddVirtualItemsToSelectMenuOptions()
            {
                List<VirtualItem> inventory = VirtualItemSystem.GetUserInventory(mongoClient, user.Id);

                if(inventory.Count == 0)
                {
                    tradeItemsSelectMenu.AddOption("Empty Inventory", "Empty Inventory");
                    return;
                }

                // foreach item from the user's inv, add it to the selectmenu options
                foreach (VirtualItem item in inventory) // !! only up to 25 options can be added to it. Update this in the future to support 25+ options somehow..
                {
                    Emote itemEmote = Emote.Parse(item.EmoteId);
                    tradeItemsSelectMenu.AddOption($"{item.Rarity} {item.Name}", item.Name, emote: itemEmote);
                }

                tradeItemsSelectMenu.MaxValues = inventory.Count;
            }

            return components;
        }




        public static Embed TradeMenuEmbed(SocketGuildUser user)
        {
            // Send a trade menu embed
            EmbedBuilder tradeMenuEmbed = new EmbedBuilder()
                .WithAuthor("Trade Menu", iconUrl: user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                .WithDescription($"{user.Mention} please select the items you would like to trade.");

            return tradeMenuEmbed.Build();
        }




        public static Trade CreateNewTrade(MongoClient mongoClient, ulong tradeThreadChannelId, 
            ulong user1Id, ulong user1TradeMenuMessageId, ulong user2Id, ulong user2TradeMenuMessageId)
        {
            var tradeCollection = mongoClient.GetDatabase("TradeDatabase").GetCollection<BsonDocument>("Trade");

            var trade = new Trade()
            {
                TradeThreadChannelId = tradeThreadChannelId,
                Trader1 = new Trader()
                {
                    GuildUserId = user1Id,
                    TradeMenuMessageId = user1TradeMenuMessageId,
                },
                Trader2 = new Trader()
                {
                    GuildUserId = user2Id,
                    TradeMenuMessageId = user2TradeMenuMessageId
                }
            };

            tradeCollection.InsertOne(trade.ToBsonDocument<Trade>());
            return trade;
        }




        public static void DeleteTrade(MongoClient mongoClient, ulong tradeThreadChannelId)
        {
            var tradeCollection = mongoClient.GetDatabase("TradeDatabase").GetCollection<BsonDocument>("Trade");
            var tradeFilter = Builders<BsonDocument>.Filter.Eq("TradeThreadChannelId", tradeThreadChannelId);
            
            tradeCollection.DeleteOne(tradeFilter);
        }




        public static Trade GetTrade(MongoClient mongoClient, ulong threadChannelId = 0, ulong tradeMessageId = 0)
        {
            var tradeCollection = mongoClient.GetDatabase("TradeDatabase").GetCollection<BsonDocument>("Trade");

            if (threadChannelId != 0)
            {
                var tradeFilter = Builders<BsonDocument>.Filter.Eq("TradeThreadChannelId", threadChannelId);

                return BsonSerializer.Deserialize<Trade>(tradeCollection.Find<BsonDocument>(tradeFilter).First());
            }

            if(tradeMessageId != 0)
            {
                var trader1Filter = Builders<BsonDocument>.Filter.Eq("Trader1.TradeMenuMessageId", tradeMessageId);
                var trader2Filter = Builders<BsonDocument>.Filter.Eq("Trader2.TradeMenuMessageId", tradeMessageId);

                Trade trade;

                try // check if messageId is from trader1
                {
                    trade = BsonSerializer.Deserialize<Trade>(tradeCollection.Find<BsonDocument>(trader1Filter).First());
                    return trade;
                }
                catch (Exception ex) // if not from trader1 it'll give an error, which means it's from trader2
                {
                    trade = BsonSerializer.Deserialize<Trade>(tradeCollection.Find<BsonDocument>(trader2Filter).First());
                    return trade;
                }
            }

            throw (new Exception("One of the Id parameters must have a value other than 0!"));
        }




        public static Trader GetTrader(MongoClient mongoClient, ulong tradeMenuMessageId)
        {
            var tradeCollection = mongoClient.GetDatabase("TradeDatabase").GetCollection<BsonDocument>("Trade");
            var trader1Filter = Builders<BsonDocument>.Filter.Eq("Trader1.TradeMenuMessageId", tradeMenuMessageId);
            var trader2Filter = Builders<BsonDocument>.Filter.Eq("Trader2.TradeMenuMessageId", tradeMenuMessageId);

            Trade trade;

            try // check if messageId is from trader1
            {
                trade = BsonSerializer.Deserialize<Trade>(tradeCollection.Find<BsonDocument>(trader1Filter).First());
                return trade.Trader1;
            }
            catch(Exception ex) // if not from trader1, it's from trader2
            {
                trade = BsonSerializer.Deserialize<Trade>(tradeCollection.Find<BsonDocument>(trader2Filter).First());
                return trade.Trader2;
            }
        }
    }
}
