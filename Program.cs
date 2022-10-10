using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ArmadilloGamingDiscordBot
{
    class Program
    {
        public static Task Main() => new Program().MainAsync();

        public async Task MainAsync()
        {

            using IHost host = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                services
                .AddSingleton(x => new DiscordSocketClient(new DiscordSocketConfig()
                {
                    GatewayIntents = GatewayIntents.AllUnprivileged,
                    AlwaysDownloadUsers = true
                }))
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<InteractionHandler>()
                ).Build();

            await RunAsync(host);
        }


        public async Task RunAsync(IHost host)
        {
            using IServiceScope serviceScope = host.Services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;

            var _client = provider.GetRequiredService<DiscordSocketClient>();
            var slashCommands = provider.GetRequiredService<InteractionService>();
            await provider.GetRequiredService<InteractionHandler>().InitializeAsync();

            _client.Log += async (LogMessage msg) => { Console.WriteLine(msg); };
            slashCommands.Log += async (LogMessage msg) => { Console.WriteLine(msg); };

            _client.Ready += async () =>
            {
                Console.WriteLine("Bot is ready!");
                await slashCommands.RegisterCommandsToGuildAsync(810214297207570452, deleteMissing: true); // test server
               // await slashCommands.RegisterCommandsToGuildAsync(892701424218148905, deleteMissing: true); // ArmadilloGaming Server
            };

            await _client.LoginAsync(TokenType.Bot, "MTAyODc1NTQyNzM4ODgyOTc0Nw.GNH56Z.cwQ1JnziVTT3QZewN-oNodIQv-O6pUHRIygVmg");
            await _client.StartAsync();

            await Task.Delay(-1);
        }
    }
}