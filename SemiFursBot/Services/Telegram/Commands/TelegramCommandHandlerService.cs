using System.Collections.Generic;

using SemiFursBot.Interfaces;
using SemiFursBot.Models;
using SemiFursBot.Services.Relay;
using SemiFursBot.Services.Relay.Services;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SemiFursBot.Services.Telegram.Commands {
    internal class TelegramCommandHandlerService {
        private readonly ILogger _logger;
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly IRelayActionTracker _relayActionTracker;
        private readonly TelegramConfig _telegramConfig;

        public TelegramCommandHandlerService(ILogger logger, ITelegramBotClient telegramBotClient,
            IRelayActionTracker relayActionTracker, TelegramConfig telegramConfig) {
            _logger = logger;
            _telegramBotClient = telegramBotClient;
            _relayActionTracker = relayActionTracker;
            _telegramConfig = telegramConfig;
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
            if (update.Message.ReplyToMessage is null
                || update.Message.Type is not MessageType.Text and not MessageType.Sticker) {
                return;
            }

            var threadId = update.Message.ReplyToMessage?.MessageThreadId ?? 0;
            var chatId = update.Message.Chat.Id;
            var channelName = update.Message.ReplyToMessage!.ForumTopicCreated.Name;
            var messageText = update.Message.Text;

            _logger.Info($"Received '{messageText}', from: [Ch:{channelName}]{update.Message.Chat.Username}");
            if (_telegramConfig.TopicNames.TryAdd(channelName, (threadId, chatId))) {
                _logger.Info($"Cached {channelName} <-> {threadId}:{chatId}");
                _telegramConfig.SaveCache();
            }

            if (update.Type == UpdateType.Message && update.Message is not null && !update.Message!.From.IsBot) {
                _relayActionTracker.AddAction(new SendMessageAction() {
                    PlatformName = "Discord",
                    ActionTime = DateTime.UtcNow,
                    ChannelName = channelName,
                    MessageContents = $"""
                    [Telegram]: {update.Message.From.Username}
                    {messageText}
                    """
                });
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
