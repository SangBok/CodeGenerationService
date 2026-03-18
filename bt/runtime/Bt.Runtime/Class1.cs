namespace Bt.Runtime;

public enum NodeStatus
{
    Running = 0,
    Success = 1,
    Failure = 2
}

public sealed record TraceEvent(
    DateTimeOffset Timestamp,
    string NodeId,
    string NodeType,
    string EventType,
    NodeStatus? Status = null,
    string? Message = null
);
