﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using ArmadilloGamingDiscordBot.Blueprints;

namespace ArmadilloGamingDiscordBot.Modules
{
    public class TradeModule : InteractionModuleBase<SocketInteractionContext>
    {
        MongoClient mongoClient = new(Storage.MongoDBConnectionString);


        [SlashCommand("trade", "Sends a trade request to the user")]
        public async Task HandleTrade([Summary("user", "The user you would like to trade with.")] SocketGuildUser user)
        {
            EmbedBuilder tradeRequestEmbed = new EmbedBuilder()
                .WithAuthor("Trade Request", iconUrl: Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl())
                .WithDescription($"{Context.User.Mention} has sent {user.Mention} a trade request.");


            await RespondAsync(embed: tradeRequestEmbed.Build(), components: TradeSystem.TradeRequestComponent(Context, user).Build());
        }


        [ComponentInteraction("traderequest_accept:*,*"),]
        public async Task HandleTradeAccept(string tradeRequesterUserId, string requestedToUserId)
        {

            SocketGuildUser tradeRequester = Context.Guild.GetUser(Convert.ToUInt64(tradeRequesterUserId));
            SocketGuildUser tradeRequestSentTo = Context.Guild.GetUser(Convert.ToUInt64(requestedToUserId));
            SocketUserMessage message = ((SocketMessageComponent)Context.Interaction).Message;


            if (Context.User.Id == tradeRequestSentTo.Id)
            {
                // Accepts the trade request & deletes buttons
                await RespondAsync($"{tradeRequestSentTo.Mention} has accepted the trade request from {tradeRequester.Mention}");
                message.ModifyAsync(msg => msg.Components = TradeSystem.TradeRequestComponent(Context, tradeRequestSentTo, true).Build());

                // Creates embed, message component, thread
                EmbedBuilder tradeInformationEmbed = new EmbedBuilder()
                    .WithAuthor("Trade Menu", iconUrl: Context.Guild.IconUrl)
                    .WithDescription($"Trade between {tradeRequester.Mention} & {tradeRequestSentTo.Mention}");

                var tradeThreadChannel = await ((ITextChannel)message.Channel).CreateThreadAsync($"Trade between {tradeRequester.Username} & {tradeRequestSentTo.Username}");

                ButtonBuilder cancelTradeButton = new ButtonBuilder()
               .WithCustomId($"trade_cancel:{tradeThreadChannel.Id}")
               .WithLabel("Cancel Trade")
               .WithStyle(ButtonStyle.Danger);

                ButtonBuilder acceptTradeButton = new ButtonBuilder()
               .WithCustomId($"trade_accept:{tradeThreadChannel.Id}")
               .WithLabel("Accept Trade")
               .WithStyle(ButtonStyle.Success);

                ComponentBuilder components = new ComponentBuilder()
                    .WithButton(cancelTradeButton)
                    .WithButton(acceptTradeButton);

                // sends trade menu for the traders to select their items for trade
                await tradeThreadChannel.SendMessageAsync(embed: tradeInformationEmbed.Build(), components: components.Build());

                IUserMessage message1 = await tradeThreadChannel.SendMessageAsync(embed: TradeSystem.TradeMenuEmbed(tradeRequester));
                await message1.ModifyAsync(message => message.Components = 
                    TradeSystem.TradeMenuComponents(mongoClient, tradeRequester, $"tradeSelectMenu:{message1.Id}").Build());

                IUserMessage message2 = await tradeThreadChannel.SendMessageAsync(embed: TradeSystem.TradeMenuEmbed(tradeRequestSentTo));
                await message2.ModifyAsync(message => message.Components =
                    TradeSystem.TradeMenuComponents(mongoClient, tradeRequestSentTo, $"tradeSelectMenu:{message2.Id}").Build());

                // adds a new trade to the database
                TradeSystem.CreateNewTrade(mongoClient, tradeThreadChannel.Id, tradeRequester.Id, message1.Id, tradeRequestSentTo.Id, message2.Id);
            }
            else
            {
                await RespondAsync("This trade request belongs to someone else.", ephemeral: true);
            }
        }


        [ComponentInteraction("traderequest_decline:*,*")]
        public async Task HandleTradeDecline(string tradeRequesterUserId, string requestedToUserId)
        {
            SocketGuildUser tradeRequester = Context.Guild.GetUser(Convert.ToUInt64(tradeRequesterUserId));
            SocketGuildUser tradeRequestSentTo = Context.Guild.GetUser(Convert.ToUInt64(requestedToUserId));
            SocketUserMessage message = ((SocketMessageComponent)Context.Interaction).Message;


            if (Context.User.Id == tradeRequestSentTo.Id)
            {
                await RespondAsync($"{tradeRequestSentTo.Mention} has declined the trade request from {tradeRequester.Mention}");
                message.ModifyAsync(msg => msg.Components = TradeSystem.TradeRequestComponent(Context,tradeRequestSentTo, true).Build());
            }
            else
            {
                await RespondAsync("This trade request belongs to someone else.", ephemeral: true);
            }
        }


        [ComponentInteraction("trade_cancel:*")]
        public async Task HandleTradeCancel(string tradeThreadChannelId)
        {
            SocketThreadChannel tradeChannel = Context.Guild.GetThreadChannel(Convert.ToUInt64(tradeThreadChannelId));
            Trade trade = TradeSystem.GetTrade(mongoClient, Convert.ToUInt64(tradeThreadChannelId));

            // Send a message saying the trade has been canceled by x user
            if (Context.User.Id == trade.Trader1.GuildUserId ||  Context.User.Id == trade.Trader2.GuildUserId)
            {
                tradeChannel.DeleteAsync();
                TradeSystem.DeleteTrade(mongoClient, Convert.ToUInt64(tradeThreadChannelId));
            }
            else
            {
                await RespondAsync("You do not have permission to close this thread.", ephemeral: true);
            }
        }


        [ComponentInteraction("tradeSelectMenu:*")]
        public async Task HandleTradeSelectMenu(string messageId, string[] inputs)
        {
            Trade trade = TradeSystem.GetTrade(mongoClient, tradeMessageId: Convert.ToUInt64(messageId));
            Trader trader = TradeSystem.GetTrader(mongoClient, Convert.ToUInt64(messageId));
            SocketGuildUser user = Context.Guild.GetUser(trader.GuildUserId);
            IThreadChannel tradeChannel = Context.Guild.GetThreadChannel(trade.TradeThreadChannelId);
            IMessage tradeMenuMessage = tradeChannel.GetMessageAsync(Convert.ToUInt64(messageId)).Result;

            if(Context.User.Id != trader.GuildUserId) { await RespondAsync("This is not your trade menu.", ephemeral: true); return; }

            string tradeItems = "";
            List<VirtualItem> virtualItemsForTrade = new List<VirtualItem>();

            foreach(string input in inputs)
            {
                VirtualItem virtualItem = VirtualItemSystem.GetItemFromDatabase(mongoClient, input);
                tradeItems += $"{virtualItem.EmoteId} ";
                virtualItemsForTrade.Add(virtualItem);

            }

            Embed newEmbedTradeMenu = TradeSystem.TradeMenuEmbed(user).ToEmbedBuilder()
                .AddField("Items for Trade", tradeItems)
                .Build();


            TradeSystem.AddItemsToTraderVirtualItems(mongoClient, Convert.ToUInt64(messageId), virtualItemsForTrade);

            // modify message with new items for trade
            await (tradeMenuMessage as IUserMessage).ModifyAsync(message =>
            {
                message.Components = new ComponentBuilder().Build();
                message.Embed = newEmbedTradeMenu;
            });
        }


        [ComponentInteraction("trade_accept:*")]
        public async Task HandleTradeAcceptButton(string tradeThreadChannelId)
        {
            SocketThreadChannel tradeChannel = Context.Guild.GetThreadChannel(Convert.ToUInt64(tradeThreadChannelId));
            Trade trade = TradeSystem.GetTrade(mongoClient, Convert.ToUInt64(tradeThreadChannelId));
            
            if (Context.User.Id == trade.Trader1.GuildUserId || Context.User.Id == trade.Trader2.GuildUserId)
            {
                Trader trader = TradeSystem.GetTrader(mongoClient, threadChannelIdAndUserId: Tuple.Create(Convert.ToUInt64(tradeThreadChannelId), Context.User.Id));

                await Context.Channel.SendMessageAsync($"{Context.User.Mention} has accepted the trade.");
                TradeSystem.SetUserTradeAccepted(mongoClient, trader);
            }
            else
            {
                await RespondAsync("You are not a part of this trade.", ephemeral: true);
            }
        }
        
    }
}
