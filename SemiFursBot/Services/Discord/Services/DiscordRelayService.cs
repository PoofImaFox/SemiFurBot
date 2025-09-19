using SemiFursBot.Interfaces;
using SemiFursBot.Services.Relay;
using SemiFursBot.Services.Relay.Actions;
using SemiFursBot.Services.Relay.Interfaces;

namespace SemiFursBot.Services.Discord.Services {
    internal class DiscordRelayService : IPlatformRelayService {
        private readonly ILogger _logger;
        private readonly IChannelLinkerService _channelLinkerService;

        public DiscordRelayService(ILogger logger, IChannelLinkerService channelLinkerService) {
            _logger = logger;
            _channelLinkerService = channelLinkerService;
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
            var discordChannel = await _channelLinkerService
                .GetDiscordChannel(messageAction.ChannelName);

            if (discordChannel is null) {
                return;
            }

            await discordChannel.SendMessageAsync(messageAction.MessageContents);
        }
    }
}
