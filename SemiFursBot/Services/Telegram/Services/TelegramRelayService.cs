
using SemiFursBot.Interfaces;
using SemiFursBot.Services.Relay;
using SemiFursBot.Services.Relay.Actions;
using SemiFursBot.Services.Relay.Interfaces;

using Telegram.Bot;

namespace SemiFursBot.Services.Telegram.Services {
    internal class TelegramRelayService : IPlatformRelayService {
        private readonly ILogger _logger;
        private readonly IChannelLinkerService _channelLinkerService;
        private readonly ITelegramBotClient _telegramBotClient;

        public TelegramRelayService(ILogger logger, IChannelLinkerService channelLinkerService,
            ITelegramBotClient telegramBotClient) {
            _logger = logger;
            _channelLinkerService = channelLinkerService;
            _telegramBotClient = telegramBotClient;
        }

        public async Task RelayAction(IRelayAction relayAction) {
            switch (relayAction) {
                case SendMessageAction messageAction:
                    await RelaySendMessage(messageAction);
                    break;
                case EditMessageAction messageAction:

                    break;
                case DeleteMessageAction messageAction:

                    break;
                case ReactionAddedAction reactionAction:

                    break;
                case ReactionRemovedAction reactionAction:

                    break;
                default:
                    throw new Exception("Could not relay this type of action!");
            }
        }

        private async Task RelaySendMessage(SendMessageAction messageAction) {
            var telegramChannelId = await _channelLinkerService.
                GetTelegramChannel(messageAction.ChannelName);

            var telegramChannel = await _telegramBotClient.GetChat(telegramChannelId);
            await _telegramBotClient.SendMessage(telegramChannel, messageAction.MessageContents);
        }
    }
}
