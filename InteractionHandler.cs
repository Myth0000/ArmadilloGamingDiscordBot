using Discord.Interactions;
using Discord.WebSocket;
using System.Reflection;

namespace ArmadilloGamingDiscordBot
{
    public class InteractionHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _commands;
        private readonly IServiceProvider _services;

        public InteractionHandler(DiscordSocketClient client, InteractionService commands, IServiceProvider services)
        {
            _client = client;
            _commands = commands;
            _services = services;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services); // adds all the interaction modules to the bot
            Console.WriteLine(_commands);
            _client.InteractionCreated += HandleInteraction;
        }

        public async Task HandleInteraction(SocketInteraction arg)
        {
            try
            {
                var ctx = new SocketInteractionContext(_client, arg);
                _commands.ExecuteCommandAsync(ctx, _services); // if command exists, it'll find it & execute it
            }
            catch(Exception ex) { Console.WriteLine(ex.ToString()); }
        }
    }
}
