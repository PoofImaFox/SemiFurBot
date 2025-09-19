
using Discord;

namespace SemiFursBot.Services.Relay.Interfaces {
    internal interface IChannelLinkerService {
        Task<ITextChannel?> GetDiscordChannel(string telegramName);
        Task<long?> GetTelegramChannel(string channelName);
    }
}