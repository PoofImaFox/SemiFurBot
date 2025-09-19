
using SemiFursBot.Services.Relay.Interfaces;

namespace SemiFursBot.Services.Relay {
    internal class SendMessageAction : IRelayAction {
        public string PlatformName { get; init; }
        public DateTime ActionTime { get; init; }
        public string MessageContents { get; set; }
        public string ChannelName { get; set; }
    }
}
