using MyPcBuild.ApiService.Domain.Models.Spatial;

namespace MyPcBuild.ApiService.Features.Spatial;

// Request Models

public record ValidatePartInstallationRequest(
    Guid ProductId,
    Guid SlotId,
    Vector3 Position
);

// Shared DTOs

public record SpatialIssueDto(
    string Message,
    string Severity,
    string Category
);
