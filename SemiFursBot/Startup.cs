using System.Reflection;

using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SemiFursBot.Interfaces;
using SemiFursBot.Models;
using SemiFursBot.Servers.Discord;
using SemiFursBot.Servers.Discord.Commands;
using SemiFursBot.Servers.Discord.Interfaces;

namespace SemiFursBot {

    internal class Startup : IStartup {
        private readonly ManualResetEvent _manualResetEvent = new(false);

        public Startup() {
        }

        public IConfiguration Configuration { get; set; } = default!;

        public void Configure(HostBuilderContext hostBuilderContext, IConfigurationBuilder configurationBuilder) {
            configurationBuilder
                .AddJsonFile("appsettings.json")
                .AddUserSecrets(Assembly.GetExecutingAssembly())
                .AddEnvironmentVariables();
        }

        public void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services) {
            SetupDiscordSingletons(out var socketClient);

            services
                .AddSingleton<LoggerConfig>()
                .AddSingleton<DiscordConfig>();

            services
                .AddSingleton(new InteractionService(socketClient.Rest))
                .AddSingleton(new CommandService())
                .AddSingleton<ICommandHandlerService, CommandHandlerService>()


                .AddHostedService<DiscordStartup>();
        }

        private void SetupDiscordSingletons(out DiscordSocketClient socketClient) {
            socketClient = new DiscordSocketClient(new DiscordSocketConfig {
                LogGatewayIntentWarnings = true,
                GatewayIntents = GatewayIntents.All ^ GatewayIntents.GuildPresences ^ GatewayIntents.GuildScheduledEvents ^ GatewayIntents.GuildInvites,
                LogLevel = LogSeverity.Verbose,
                MaxWaitBetweenGuildAvailablesBeforeReady = 250,
            });

            socketClient.Ready += SocketClient_Ready;

            var discordConfig = new DiscordUserConfig(Configuration);
            socketClient.LoginAsync(TokenType.Bot, discordConfig.DiscordToken).Wait();
            socketClient.StartAsync().Wait();

            Console.WriteLine("Waiting for discord gateway ready.");
            _manualResetEvent.WaitOne();

            Console.WriteLine("Discord gateway is ready...");
        }

        private Task SocketClient_Ready() {
            _manualResetEvent.Set();
            return Task.CompletedTask;
        }
    }
}
