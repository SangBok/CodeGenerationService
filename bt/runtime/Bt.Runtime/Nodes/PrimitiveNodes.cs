namespace Bt.Runtime.Nodes;

internal sealed class DelayNode : INode
{
    private sealed class State
    {
        public bool Started;
        public DateTimeOffset? FinishAt;
    }

    public DelayNode(string id, TimeSpan delay)
    {
        Id = id;
        _delay = delay;
    }

    public string Id { get; }
    public string Type => "Action.Delay";
    private readonly TimeSpan _delay;

    public async Task<NodeStatus> TickAsync(BtContext context)
    {
        var state = context.GetOrCreateState(Id, () => new State());
        if (!state.Started)
        {
            state.Started = true;
            state.FinishAt = context.Clock.UtcNow + _delay;
        }

        if (context.Clock.UtcNow >= state.FinishAt)
        {
            state.Started = false;
            state.FinishAt = null;
            return NodeStatus.Success;
        }

        // keep responsive to cancellation
        await context.Clock.Delay(TimeSpan.FromMilliseconds(5), context.CancellationToken).ConfigureAwait(false);
        return NodeStatus.Running;
    }
}

internal sealed class AlwaysTrueNode : INode
{
    public AlwaysTrueNode(string id) => Id = id;
    public string Id { get; }
    public string Type => "Condition.AlwaysTrue";
    public Task<NodeStatus> TickAsync(BtContext context) => Task.FromResult(NodeStatus.Success);
}

internal sealed class AlwaysFalseNode : INode
{
    public AlwaysFalseNode(string id) => Id = id;
    public string Id { get; }
    public string Type => "Condition.AlwaysFalse";
    public Task<NodeStatus> TickAsync(BtContext context) => Task.FromResult(NodeStatus.Failure);
}

