using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using ArmadilloGamingDiscordBot.Blueprints;

namespace ArmadilloGamingDiscordBot.Modules
{

    public class SettingsModule : InteractionModuleBase<SocketInteractionContext>
    {
        MongoClient mongoClient = new(Storage.MongoDBConnectionString);

        [SlashCommand("settings", "Displays the current settings of the bot.")]
        public async Task HandleSettings()
        {
            await DeferAsync();
            var settingsCollection = mongoClient.GetDatabase("SettingsDatabase").GetCollection<BsonDocument>("Settings");
            Settings settings = BsonSerializer.Deserialize<Settings>(settingsCollection.Find<BsonDocument>(new BsonDocument()).First());


            Embed settingsEmbed = new EmbedBuilder()
                .WithAuthor("Settings", iconUrl: Context.Guild.IconUrl)
                .AddField("Level Up Messages Channel", $"{GuildEmotes.armadillo} {RankSystem.GetLevelUpMessagesTextChannel(mongoClient, Context.Guild).Mention}")
                .AddField(new EmbedFieldBuilder() { Name = "Exp Gain Cooldown", Value = $"{GuildEmotes.armadillo} {settings.ExpGainCooldown} seconds" })
                .WithCurrentTimestamp()
                .Build();

            await FollowupAsync(embed: settingsEmbed);
        }




        [DefaultMemberPermissions(GuildPermission.Administrator)]
        [SlashCommand("exp-cooldown", "Allows the admin to change the cooldown for gaining exp.")]
        public async Task HandleExpCooldown(int cooldown)
        {
            var settingsCollection = mongoClient.GetDatabase("SettingsDatabase").GetCollection<BsonDocument>("Settings");
            var updateExpGainCooldown = Builders<BsonDocument>.Update.Set("ExpGainCooldown", cooldown);

            settingsCollection.UpdateOne(new BsonDocument(), updateExpGainCooldown);

            await RespondAsync($"{GuildEmotes.armadillo} The exp gain cooldown has been set to **{cooldown}** seconds.", ephemeral: true);
        }




        [DefaultMemberPermissions(GuildPermission.Administrator)]
        [SlashCommand("levelup-messages-channel", "Changes the text channel for level up messages.")]
        public async Task HandleLevelUpMessagesChannel(SocketTextChannel levelup_messages_channel)
        {
            var settingsCollection = mongoClient.GetDatabase("SettingsDatabase").GetCollection<BsonDocument>("Settings");
            Settings settings = BsonSerializer.Deserialize<Settings>(settingsCollection.Find<BsonDocument>(new BsonDocument()).First());

            try
            {
                UpdateLevelUpMessagesChannel();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            await RespondAsync($"{levelup_messages_channel.Mention} has been set as the Level Up Messages Channel.", ephemeral:true);

            void UpdateLevelUpMessagesChannel()
            {
                // updates the channel in database
                settings.LevelUpMessagesChannelId = levelup_messages_channel.Id;
                var updateLevelUpMessagesChannel = Builders<BsonDocument>.Update.Set("LevelUpMessagesChannelId", settings.LevelUpMessagesChannelId);
                settingsCollection.UpdateOne(new BsonDocument(), updateLevelUpMessagesChannel);
            }
        }
    }
}
