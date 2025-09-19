using SemiFursBot.Services.Relay.Interfaces;

namespace SemiFursBot.Services.Relay.Services {
    internal class RelayActionTracker : IRelayActionTracker {
        private readonly Queue<IRelayAction> _relayActions = new();

        public RelayActionTracker() {

        }

        public void AddAction(IRelayAction action) {
            _relayActions.Enqueue(action);
        }

        public IRelayAction GetAction() {
            return _relayActions.Dequeue();
        }

        public bool TryDequeue(out IRelayAction relayAction) {
            return _relayActions.TryDequeue(out relayAction);
        }
    }
}
