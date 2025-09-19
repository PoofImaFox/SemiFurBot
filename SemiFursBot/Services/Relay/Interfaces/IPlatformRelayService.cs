namespace SemiFursBot.Services.Relay.Interfaces {
    internal interface IPlatformRelayService {
        Task RelayAction(IRelayAction relayAction);
    }
}