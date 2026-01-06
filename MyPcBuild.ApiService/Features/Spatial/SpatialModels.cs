using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Domain.Models.Spatial;

namespace MyPcBuild.ApiService.Features.Spatial;

// Request Models

public record ValidatePartInstallationRequest(
    Guid ChamberId,
    Guid SlotId,
    Guid ProductId,
    Vector3 Position,
    Dimensions Dimensions
);

public record ConfigureChamberRequest(
    string Name,
    Dimensions Dimensions
);

public record AddSlotRequest(
    Guid ChamberId,
    string SlotName,
    ProductCategory AllowedCategory,
    Vector3 RelativePosition,
    Dimensions MaxDimensions,
    Guid? ParentSlotId = null
);

public record InstallPartRequest(
    Guid ChamberId,
    Guid SlotId,
    Guid ProductId,
    Vector3 Position,
    Dimensions Dimensions
);

// Response Models

public record SpatialIssueDto(
    string Message,
    string Severity,
    string Category
);

public record SpatialValidationResponse(
    bool IsValid,
    bool HasErrors,
    bool HasWarnings,
    List<SpatialIssueDto> Issues
);

public record ChamberDto(
    Guid Id,
    string Name,
    DimensionsDto Dimensions,
    List<SlotDto> Slots,
    List<InstalledPartDto> InstalledParts
);

public record DimensionsDto(
    decimal Length,
    decimal Width,
    decimal Height
);

public record Vector3Dto(
    decimal X,
    decimal Y,
    decimal Z
);

public record SlotDto(
    Guid Id,
    string Name,
    string AllowedCategory,
    Vector3Dto RelativePosition,
    DimensionsDto MaxDimensions,
    Guid? InstalledPartId,
    List<SlotDto> SubSlots
);

public record InstalledPartDto(
    Guid Id,
    Guid ProductId,
    Guid SlotId,
    Vector3Dto Position,
    DimensionsDto Dimensions
);
