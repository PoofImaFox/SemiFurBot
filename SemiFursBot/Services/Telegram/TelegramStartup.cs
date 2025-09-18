
using Microsoft.Extensions.Hosting;

using SemiFursBot.Services.Telegram.Commands;

namespace SemiFursBot.Services.Telegram {
    internal class TelegramStartup : IHostedService {
        private readonly TelegramCommandHandlerService _telegramCommandHandlerService;

        public TelegramStartup(TelegramCommandHandlerService telegramCommandHandlerService) {
            _telegramCommandHandlerService = telegramCommandHandlerService;
        }
        public async Task StartAsync(CancellationToken cancellationToken) {
            await _telegramCommandHandlerService.InitializeAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }
    }
}
