using SemiFursBot.Services.Relay.Interfaces;

namespace SemiFursBot.Services.Relay.Actions {
    internal class DeleteMessageAction : IRelayAction {
        public string PlatformName { get; init; }
        public DateTime ActionTime { get; init; }
        public ulong MessageId { get; set; }
    }
}
