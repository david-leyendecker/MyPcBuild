using MyPcBuild.ApiService.SharedDomain.Spatial;

namespace MyPcBuild.ApiService.Builds.Events;

public abstract record BuildEvent
{
    public Guid BuildId { get; init; }
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}

public record BuildCreated : BuildEvent
{
    public required string Name { get; init; }
    public required Guid UserId { get; init; }
}

public record PartAdded : BuildEvent
{
    public required Guid ProductId { get; init; }
    public required decimal PricePaid { get; init; }
}

public record PartAddedToSlot : BuildEvent
{
    public required Guid ProductId { get; init; }
    public required decimal PricePaid { get; init; }
    public required Guid SlotId { get; init; }
    public required Vector3 Position { get; init; }
    public Rotation? Rotation { get; init; }
}

public record PartRemoved : BuildEvent
{
    public required Guid ProductId { get; init; }
}

public record BuildRenamed : BuildEvent
{
    public required string NewName { get; init; }
}
