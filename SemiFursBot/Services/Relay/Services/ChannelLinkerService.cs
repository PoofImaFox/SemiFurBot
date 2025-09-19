using System.Text.RegularExpressions;

using Discord;
using Discord.WebSocket;

using SemiFursBot.Interfaces;
using SemiFursBot.Models;
using SemiFursBot.Services.Relay.Interfaces;

using Telegram.Bot;

namespace SemiFursBot.Services.Relay.Services {
    internal class ChannelLinkerService : IChannelLinkerService {
        private readonly Regex _symbolsRegex = new(@"[^a-zA-Z0-9]");
        private readonly ILogger _logger;
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly IDiscordClient _discordClient;
        private readonly TelegramConfig _telegramConfig;

        public ChannelLinkerService(ILogger logger, ITelegramBotClient telegramBotClient,
            DiscordSocketClient discordClient, TelegramConfig telegramConfig) {
            _logger = logger;
            _telegramBotClient = telegramBotClient;
            _discordClient = discordClient;
            _telegramConfig = telegramConfig;
        }

        public async Task<ITextChannel?> GetDiscordChannel(string channelName) {
            var semifursGuild = await _discordClient.GetGuildAsync(1406438666435297291);

            var channels = await semifursGuild.GetTextChannelsAsync();
            var cleanedName = _symbolsRegex.Replace(channelName, string.Empty);

            if (channels.FirstOrDefault(i => _symbolsRegex.Replace(i.Name, string.Empty)
                .Equals(cleanedName, StringComparison.OrdinalIgnoreCase)) is not ITextChannel channel) {
                return null;
            }

            return channel;
        }

        public async Task<(int, long)?> GetTelegramChannel(string channelName) {
            var cleanedName = _symbolsRegex.Replace(channelName, string.Empty);

            if (_telegramConfig.TopicNames.FirstOrDefault(i => _symbolsRegex.Replace(i.Key, string.Empty)
                .Equals(cleanedName, StringComparison.OrdinalIgnoreCase)).Value is (int threadID, long chatId)) {
                return (threadID, chatId);
            }

            return null;
        }
    }
}
