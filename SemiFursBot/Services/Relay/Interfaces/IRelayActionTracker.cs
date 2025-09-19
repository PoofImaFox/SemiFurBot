using SemiFursBot.Services.Relay.Interfaces;

namespace SemiFursBot.Services.Relay.Services {
    internal interface IRelayActionTracker {
        void AddAction(IRelayAction action);
        IRelayAction GetAction();
        bool TryDequeue(out IRelayAction relayAction);
    }
}