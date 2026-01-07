using MyPcBuild.ApiService.Domain.Models.Spatial;

namespace MyPcBuild.ApiService.Features.Spatial;

// Request Models

public record ValidatePartInstallationRequest(
    Guid ProductId,
    Guid SlotId,
    Vector3 Position
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
