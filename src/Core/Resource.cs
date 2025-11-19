using System.ComponentModel;

namespace VictorFrye.CrudCounter.Core;

public record Resource
{
    [Description("The unique identifier of the resource.")]
    public Guid Id { get; init; } = Guid.CreateVersion7();

    [Description("The display name of the resource.")]
    public required string Name { get; set; }

    [Description("The current count of the resource.")]
    public int Count { get; set; }
}
