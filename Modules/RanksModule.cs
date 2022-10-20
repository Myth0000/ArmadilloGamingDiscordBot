using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord.WebSocket;
using Discord;
using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace ArmadilloGamingDiscordBot.Modules
{
    public class RanksModule : InteractionModuleBase<SocketInteractionContext>
    {
        private MongoClient mongoClient = new(Storage.TestBotDatabaseConnectionString);

        [SlashCommand("level", "Displays the user's current rank.")]
        public async Task HandleRank(SocketUser user = null)
        {
            if(user == null) { user = Context.User; }
            try
            {
                await RespondAsync(embed: BuildRankEmbed());
            }
            catch (InvalidOperationException ex) // user doesn't exist in database
            {
                RankSystem.CreateNewUser(mongoClient, user.Id);
                await RespondAsync(embed: BuildRankEmbed());
            }

            Embed BuildRankEmbed()
            {
                var userCollection = mongoClient.GetDatabase("UserDatabase").GetCollection<BsonDocument>("User");
                var userFilter = Builders<BsonDocument>.Filter.Eq("UserId", user.Id);

                Rank userRank = BsonSerializer.Deserialize<User>(userCollection.Find<BsonDocument>(userFilter).First()).Rank;

                return new EmbedBuilder()
                    .WithAuthor($"{user.Username}#{user.Discriminator}", iconUrl: user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                    .AddField(new EmbedFieldBuilder() { Name = $"Level {userRank.Level}", Value = $"{GuildEmotes.armadillo} {userRank.CurrentExp}/{userRank.MaxExp}" })
                    .AddField(new EmbedFieldBuilder() { Name = "Total Exp", Value = $"{GuildEmotes.armadillo} {userRank.TotalExp}" })
                    .WithCurrentTimestamp()
                    .Build();
            }
        }




        [SlashCommand("leaderboard", "Shows a list of the top 10 highest level members in the server.")]
        public async Task HandleLeaderboard()
        {
            var userCollection = mongoClient.GetDatabase("UserDatabase").GetCollection<BsonDocument>("User");
            var sortLevel = Builders<BsonDocument>.Sort.Descending("Rank.TotalExp");
            var userBsonList = userCollection.Find(new BsonDocument()).Sort(sortLevel).Limit(10).ToList();
            var users = userBsonList.Select(item => BsonSerializer.Deserialize<User>(item)).ToList();
            string topTenHighestLeveledUsers = "";

            for(int index = 0; index < users.Count; index++)
            {
                if(index == users.Count - 1)
                {
                    topTenHighestLeveledUsers += $"**{index + 1}.** {Context.Guild.GetUser(users[index].UserId).Mention} {GuildEmotes.armadillo} {users[index].Rank.Level}";
                    break;
                }
                topTenHighestLeveledUsers += $"**{index + 1}.** {Context.Guild.GetUser(users[index].UserId).Mention} {GuildEmotes.armadillo} {users[index].Rank.Level}\n";
            }


            EmbedBuilder leaderboardEmbed = new EmbedBuilder()
                .WithAuthor("Leaderboard", iconUrl: Context.Guild.IconUrl)
                .AddField(new EmbedFieldBuilder() { Name=$"Top {users.Count}", Value= topTenHighestLeveledUsers })
                .WithCurrentTimestamp();

            await RespondAsync(embed: leaderboardEmbed.Build());
        }




        [RequireUserPermission(GuildPermission.Administrator)]
        [SlashCommand("exp-cooldown", "Allows the admin to change the cooldown for gaining exp.")]
        public async Task HandleExpCooldown(int cooldown)
        {
            var settingsCollection = mongoClient.GetDatabase("SettingsDatabase").GetCollection<BsonDocument>("Settings");
            var updateExpGainCooldown = Builders<BsonDocument>.Update.Set("ExpGainCooldown", cooldown);

            settingsCollection.UpdateOne(new BsonDocument(), updateExpGainCooldown);

            await RespondAsync($"{GuildEmotes.armadillo} The exp gain cooldown has been set to **{cooldown}** seconds.", ephemeral:true);
        }



        [SlashCommand("settings", "Displays the current settings of the bot.")]
        public async Task HandleSettings()
        {
            var settingsCollection = mongoClient.GetDatabase("SettingsDatabase").GetCollection<BsonDocument>("Settings");
            Settings settings = BsonSerializer.Deserialize<Settings>(settingsCollection.Find<BsonDocument>(new BsonDocument()).First());

            Embed settingsEmbed = new EmbedBuilder()
                .WithAuthor("Settings", iconUrl: Context.Guild.IconUrl)
                .AddField(new EmbedFieldBuilder() { Name = "Exp Gain Cooldown", Value = $"{GuildEmotes.armadillo} {settings.ExpGainCooldown} seconds"})
                .WithCurrentTimestamp()
                .Build();

            await RespondAsync(embed: settingsEmbed);
        }
    }
}
