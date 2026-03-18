namespace Bt.Runtime.Nodes;

internal sealed class TimeoutDecoratorNode : INode
{
    private sealed class State
    {
        public DateTimeOffset? Start;
    }

    public TimeoutDecoratorNode(string id, INode child, TimeSpan timeout)
    {
        Id = id;
        _child = child;
        _timeout = timeout;
    }

    public string Id { get; }
    public string Type => "Decorator.Timeout";
    private readonly INode _child;
    private readonly TimeSpan _timeout;

    public async Task<NodeStatus> TickAsync(BtContext context)
    {
        var state = context.GetOrCreateState(Id, () => new State());
        state.Start ??= context.Clock.UtcNow;

        if (context.Clock.UtcNow - state.Start.Value > _timeout)
        {
            state.Start = null;
            context.Trace?.Invoke(new TraceEvent(context.Clock.UtcNow, Id, Type, "timeout", NodeStatus.Failure));
            return NodeStatus.Failure;
        }

        var status = await _child.TickAsync(context).ConfigureAwait(false);
        if (status != NodeStatus.Running) state.Start = null;
        return status;
    }
}

