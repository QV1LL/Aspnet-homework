namespace ErrorHandlingSample.Helpers;

public record ProblemDetails
{
    public required string Type  { get; init; }
    public required string Title { get; init; }
    public required int Status  { get; init; }
    public required string Detail { get; init; }
    public required string Instance { get; init; }
    public required string TraceId { get; init; }
    public DateTimeOffset Timestamp { get; init; }
}