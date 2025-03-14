using System.Collections.Frozen;

namespace VictorFrye.SimpleCrud.ApiService;

public static class WellKnownResources
{
    public static readonly Resource Food = new()
    {
        Id = Guid.Parse("0195725a-eb24-7f14-8b2f-d131671684f1"),
        Name = nameof(Food),
        Count = 200
    };

    public static readonly Resource Wood = new()
    {
        Id = Guid.Parse("0195725b-5031-72cc-8969-16dd38932381"),
        Name = nameof(Wood),
        Count = 200
    };

    public static readonly Resource Gold = new()
    {
        Id = Guid.Parse("0195725b-b77e-7ed0-9387-9dca428f3337"),
        Name = nameof(Gold),
        Count = 100
    };

    public static readonly Resource Stone = new()
    {
        Id = Guid.Parse("0195725b-ed2f-7842-8305-ceb37b249472"),
        Name = nameof(Stone),
        Count = 100
    };

    public static readonly FrozenSet<Resource> StartingResources = [Food, Wood, Gold, Stone];
}
