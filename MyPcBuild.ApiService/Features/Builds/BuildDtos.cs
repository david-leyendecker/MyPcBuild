namespace MyPcBuild.ApiService.Features.Builds;

/// <summary>
/// Shared DTOs for build-related spatial data
/// </summary>

public record Vector3Dto(
    decimal X,
    decimal Y,
    decimal Z
);

public record DimensionsDto(
    decimal Length,
    decimal Width,
    decimal Height
);

public record SlotDto(
    Guid Id,
    string Name,
    string AllowedCategory,
    Vector3Dto RelativePosition,
    DimensionsDto MaxDimensions
);

public record ChamberDto(
    Guid Id,
    string Name,
    DimensionsDto Dimensions,
    List<SlotDto> Slots
);
