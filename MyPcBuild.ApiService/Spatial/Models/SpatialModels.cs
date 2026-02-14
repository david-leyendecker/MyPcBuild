using MyPcBuild.ApiService.SharedDomain.Spatial;

namespace MyPcBuild.ApiService.Spatial.Models;

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
