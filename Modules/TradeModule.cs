using System;
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

namespace ArmadilloGamingDiscordBot.Modules
{
    public class TradeModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("trade", "Sends a trade request to the user")]
        public async Task HandleTrade([Summary("user", "The user you would like to trade with.")] SocketGuildUser user)
        {
            EmbedBuilder tradeRequestEmbed = new EmbedBuilder()
                .WithAuthor("Trade Request", iconUrl: Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl())
                .WithDescription($"{Context.User.Mention} has sent {user.Mention} a trade request.");


            await RespondAsync(embed: tradeRequestEmbed.Build(), components: TradeRequestComponent(user).Build());
        }



        [ComponentInteraction("trade_accept:*,*"),]
        public async Task HandleTradeAccept(string tradeRequesterUserId, string requestedToUserId)
        {

            SocketGuildUser tradeRequester = Context.Guild.GetUser(Convert.ToUInt64(tradeRequesterUserId));
            SocketGuildUser tradeRequestSentTo = Context.Guild.GetUser(Convert.ToUInt64(requestedToUserId));
            SocketUserMessage message = ((SocketMessageComponent)Context.Interaction).Message;
            string selectMenuCustomId = $"tradeItemsSelectMenu:{tradeRequester.Id},{tradeRequestSentTo.Id}";
            string buttonCustomId = $"tradeItemsButton:{tradeRequester.Id},{tradeRequestSentTo.Id}";


            if (Context.User.Id == tradeRequestSentTo.Id)
            {
                // Accepts the trade request & deletes buttons
                await RespondAsync($"{tradeRequestSentTo.Mention} has accepted the trade request from {tradeRequester.Mention}");
                message.ModifyAsync(msg => msg.Components = TradeRequestComponent(tradeRequestSentTo, true).Build());

                // Send a trade menu embed
                EmbedBuilder tradeMenuEmbed = new EmbedBuilder()
                    .WithAuthor("Trade Menu", iconUrl: Context.Guild.IconUrl)
                    .WithDescription($"Trade between {tradeRequester.Mention} & {tradeRequestSentTo.Mention}");

                var tradeThreadChannel = await ((ITextChannel)message.Channel).CreateThreadAsync($"Trade between {tradeRequester.Username} & {tradeRequestSentTo.Username}");
                tradeThreadChannel.SendMessageAsync(embed: tradeMenuEmbed.Build(), components: TradeMenuComponents(selectMenuCustomId, buttonCustomId).Build());

            }
            else
            {
                await RespondAsync("This trade request belongs to someone else.", ephemeral: true);
            }
        }


        [ComponentInteraction("trade_decline:*,*")]
        public async Task HandleTradeDecline(string tradeRequesterUserId, string requestedToUserId)
        {
            SocketGuildUser tradeRequester = Context.Guild.GetUser(Convert.ToUInt64(tradeRequesterUserId));
            SocketGuildUser tradeRequestSentTo = Context.Guild.GetUser(Convert.ToUInt64(requestedToUserId));
            SocketUserMessage message = ((SocketMessageComponent)Context.Interaction).Message;


            if (Context.User.Id == tradeRequestSentTo.Id)
            {
                await RespondAsync($"{tradeRequestSentTo.Mention} has declined the trade request from {tradeRequester.Mention}");
                message.ModifyAsync(msg => msg.Components = TradeRequestComponent(tradeRequestSentTo, true).Build());
            }
            else
            {
                await RespondAsync("This trade request belongs to someone else.", ephemeral: true);
            }
        }




        ////////// COMPONENTS \\\\\\\\\\

        private ComponentBuilder TradeRequestComponent(SocketGuildUser tradeRequestSentTo, bool disableButtons = false)
        {
            ButtonBuilder acceptTradeRequestButton = new ButtonBuilder()
                .WithCustomId($"trade_accept:{Context.User.Id},{tradeRequestSentTo.Id}")
                .WithLabel("Accept")
                .WithStyle(ButtonStyle.Success);

            ButtonBuilder declineTradeRequestButton = new ButtonBuilder()
                .WithCustomId($"trade_decline:{Context.User.Id},{tradeRequestSentTo.Id}")
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




        private ComponentBuilder TradeMenuComponents(string selectMenuCustomId, string buttonCustomId)
        {
            // Add a drop down box as a item-selection tool if possible ( this might not work because of discord )
            // If that fails, add a command that'll let users add trade items to the menu
            SelectMenuBuilder tradeItemsSelectMenu = new SelectMenuBuilder()
                .WithCustomId(selectMenuCustomId)
                .WithMaxValues(1)
                .WithMinValues(0)
                .WithPlaceholder("Select a Virtual Item to trade")
                .AddOption("Virtual Item", "Virtual Item", "An item from your inventory.");

            ButtonBuilder cancelTradeButton = new ButtonBuilder()
                .WithCustomId(buttonCustomId)
                .WithLabel("Cancel Trade")
                .WithStyle(ButtonStyle.Danger);

            ComponentBuilder components = new ComponentBuilder()
                .WithSelectMenu(tradeItemsSelectMenu)
                .WithButton(cancelTradeButton);

            return components;
        }
    }
}
