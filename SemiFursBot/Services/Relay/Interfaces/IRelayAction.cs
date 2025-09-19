namespace SemiFursBot.Services.Relay.Interfaces {
    internal interface IRelayAction {
        string PlatformName { get; init; }
        DateTime ActionTime { get; init; }
    }
}