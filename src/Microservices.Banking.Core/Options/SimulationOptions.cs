namespace Microservices.Banking.Core.Options;

public sealed class SimulationOptions
{
    public int MinDelayBeforeStatusChanged { get; init; }
    public int MaxDelayBeforeStatusChanged { get; init; }
    public int FailureChance { get; init; }
}