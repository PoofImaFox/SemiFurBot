
using Microsoft.Extensions.Hosting;

using SemiFursBot.Services.Discord.Services;
using SemiFursBot.Services.Relay.Interfaces;
using SemiFursBot.Services.Relay.Services;
using SemiFursBot.Services.Telegram.Services;

namespace SemiFursBot.Services.Relay {
    internal class RelayService : IHostedService {
        private readonly IRelayActionTracker _relayActionTracker;
        private readonly IPlatformRelayService _discordRelayService;
        private readonly IPlatformRelayService _telegramRelayService;
        private Task _relayWorker;

        public RelayService(IRelayActionTracker relayActionTracker, DiscordRelayService discordRelayService,
            TelegramRelayService telegramRelayService) {
            _telegramRelayService = telegramRelayService;
            _relayActionTracker = relayActionTracker;
            _discordRelayService = discordRelayService;
        }

        public Task StartAsync(CancellationToken cancellationToken) {
            _relayWorker = Task.Run(RelayWorker);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        private async Task RelayWorker() {
            while (true) {
                if (!_relayActionTracker.TryDequeue(out var relayAction)) {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    continue;
                }

                switch (relayAction.PlatformName) {
                    case "Telegram":
                        await _telegramRelayService.RelayAction(relayAction);
                        break;
                    case "Discord":
                        await _discordRelayService.RelayAction(relayAction);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
