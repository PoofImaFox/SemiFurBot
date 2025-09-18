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
using SemiFursBot.Services.Discord;
using SemiFursBot.Services.Discord.Commands;
using SemiFursBot.Services.Discord.Interfaces;
using SemiFursBot.Services.Telegram;
using SemiFursBot.Services.Telegram.Commands;

using Telegram.Bot;

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

            AddTelegramClient(services);

            services
                .AddSingleton<LoggerConfig>()
                .AddSingleton<DiscordConfig>();

            services
                .AddSingleton<ILogger, Logger>()

                .AddSingleton(socketClient)
                .AddSingleton(new InteractionService(socketClient.Rest))
                .AddSingleton(new CommandService())
                .AddSingleton<ICommandHandlerService, DiscordCommandHandlerService>()

                .AddHostedService<DiscordStartup>();
        }

        private void AddTelegramClient(IServiceCollection services) {
            var telegramConfig = new TelegramConfig(Configuration);
            var telegramClient = new TelegramBotClient(telegramConfig.Token);
            services
                .AddSingleton<ITelegramBotClient>(telegramClient)
                .AddSingleton<TelegramCommandHandlerService>()
                .AddHostedService<TelegramStartup>();
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
