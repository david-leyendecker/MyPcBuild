using Marten;
using MyPcBuild.ApiService.Domain.Events;
using MyPcBuild.ApiService.Domain.Models.Spatial;

namespace MyPcBuild.ApiService.Domain.Models;

public class Build
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public List<BuildPart> Parts { get; set; } = [];
    public int Version { get; set; }

    public void Apply(BuildCreated @event)
    {
        Id = @event.BuildId;
        Name = @event.Name;
        UserId = @event.UserId;
    }

    public void Apply(PartAdded @event)
    {
        Parts.Add(new BuildPart(@event.ProductId, @event.PricePaid, null, null));
    }

    public void Apply(PartAddedToSlot @event)
    {
        Parts.Add(new BuildPart(
            @event.ProductId,
            @event.PricePaid,
            @event.SlotId,
            @event.Position
        ));
    }

    public void Apply(PartRemoved @event)
    {
        Parts.RemoveAll(p => p.ProductId == @event.ProductId);
    }

    public void Apply(BuildRenamed @event)
    {
        Name = @event.NewName;
    }
}

public record BuildPart(
    Guid ProductId,
    decimal PricePaid,
    Guid? SlotId = null,
    Vector3? Position = null
);
