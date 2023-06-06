using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace ArmadilloGamingDiscordBot
{
    class Program
    {
        public static Task Main() => new Program().MainAsync();

        public async Task MainAsync()
        {
            try
            {
                using IHost host = Host.CreateDefaultBuilder()
                    .ConfigureServices((_, services) =>
                    services
                    .AddSingleton(x => new DiscordSocketClient(new DiscordSocketConfig()
                    {
                        GatewayIntents = GatewayIntents.All,
                        AlwaysDownloadUsers = true
                    }))
                    .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                    .AddSingleton<InteractionHandler>()
                    ).Build();

                await RunAsync(host);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        public async Task RunAsync(IHost host)
        {
            try
            {
                ClientEvents clientEvents = new();

                using IServiceScope serviceScope = host.Services.CreateScope();
                IServiceProvider provider = serviceScope.ServiceProvider;

                var _client = provider.GetRequiredService<DiscordSocketClient>();
                var slashCommands = provider.GetRequiredService<InteractionService>();
                await provider.GetRequiredService<InteractionHandler>().InitializeAsync();

                _client.Log += async (LogMessage msg) => { Console.WriteLine(msg); };
                slashCommands.Log += async (LogMessage msg) => { Console.WriteLine(msg); };
                _client.MessageReceived += clientEvents.MessageRecievedEvent;
                _client.UserLeft += clientEvents.UserLeftGuild;

                _client.Ready += async () =>
                {
                    Console.WriteLine("Bot is ready!");
                    await slashCommands.RegisterCommandsToGuildAsync(Storage.DiscordServerId, deleteMissing: true); // current server
                    await slashCommands.RegisterCommandsToGuildAsync(1036338309410082977, deleteMissing: true); // emoji server

                    RemoveUnexistingUsersFromDatabase(_client);
                };
 
                await _client.LoginAsync(TokenType.Bot, Storage.DiscordBotToken);

                await _client.StartAsync();

                await Task.Delay(-1);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void RemoveUnexistingUsersFromDatabase(DiscordSocketClient client)
        {
            MongoClient mongoClient = new(Storage.MongoDBConnectionString);
            var collection = mongoClient.GetDatabase("UserDatabase").GetCollection<BsonDocument>("User");
            var databaseUsers = collection.Find<BsonDocument>(new BsonDocument()).ToList().Select(_user => BsonSerializer.Deserialize<User>(_user));
            List<SocketGuildUser> users = client.Guilds.First(x => x.Id == Storage.DiscordServerId).Users.ToList();

            foreach (var databaseUser in databaseUsers)
            {
                bool userExists = false;

                foreach(var guildUser in users)
                {
                    if(databaseUser.UserId == guildUser.Id) { userExists = true; continue; }
                }

                if(!userExists)
                {
                    var filterUser = Builders<BsonDocument>.Filter.Eq("UserId", databaseUser.UserId);
                    collection.DeleteOne(filterUser);
                    Console.WriteLine($"{databaseUser.UserId} has been removed from the database");
                }

            }


        }
    }
}