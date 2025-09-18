
using Microsoft.Extensions.Hosting;

using SemiFursBot.Interfaces;
using SemiFursBot.Services.Discord.Interfaces;

namespace SemiFursBot.Services.Discord {
    internal class DiscordStartup : IHostedService {
        private readonly ICommandHandlerService _commandHandlerService;
        private readonly ILogger _logger;

        public DiscordStartup(ICommandHandlerService commandHandlerService, ILogger logger) {
            _commandHandlerService = commandHandlerService;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken) {
            _logger.Info("Starting discord bot services.");
            await _commandHandlerService.InitializeAsync();

            _logger.Info("Running");
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            return Task.CompletedTask;
        }
    }
}
