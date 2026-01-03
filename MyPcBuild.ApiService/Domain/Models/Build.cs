using Marten;
using MyPcBuild.ApiService.Domain.Events;

namespace MyPcBuild.ApiService.Domain.Models;

public class Build
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public List<BuildPart> Parts { get; set; } = new();
    public int Version { get; set; }

    public void Apply(BuildCreated @event)
    {
        Id = @event.BuildId;
        Name = @event.Name;
        UserId = @event.UserId;
    }

    public void Apply(PartAdded @event)
    {
        Parts.Add(new BuildPart(@event.ProductId, @event.PricePaid));
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

public record BuildPart(Guid ProductId, decimal PricePaid);
