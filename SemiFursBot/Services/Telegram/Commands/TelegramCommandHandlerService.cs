using SemiFursBot.Interfaces;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SemiFursBot.Services.Telegram.Commands {
    internal class TelegramCommandHandlerService {
        private readonly ILogger _logger;
        private readonly ITelegramBotClient _telegramBotClient;

        public TelegramCommandHandlerService(ILogger logger, ITelegramBotClient telegramBotClient) {
            _logger = logger;
            _telegramBotClient = telegramBotClient;
        }

        public async Task InitializeAsync() {
            var receiverOptions = new ReceiverOptions {
                AllowedUpdates = new[] {
                    UpdateType.Message
                }
            };

            //await _telegramBotClient.SetMyCommands(_commandResolvingService.BotCommands);
            //_logger.Info($"Added {_commandResolvingService.BotCommands.Length} commands.");

            _telegramBotClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions);

            _logger.Info($"Telegram bot in listening mode.");
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) {
            if (update.Message.Type is not MessageType.Text and not MessageType.Sticker) {
                return;
            }

            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;

            _logger.Info($"Received '{messageText}', from: [Ch:{chatId}]{update.Message.Chat.Username}");
            if (update.Type == UpdateType.Message && update.Message is not null) {
                //if (!_commandResolvingService.InvokeCommand(update.Message)) {
                //}
            }

            return;
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken) {
            var ErrorMessage = exception switch {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            _logger.Error(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
