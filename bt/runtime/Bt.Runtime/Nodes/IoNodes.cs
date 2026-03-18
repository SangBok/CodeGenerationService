namespace Bt.Runtime.Nodes;

internal sealed class SetOutputNode : INode
{
    public SetOutputNode(string id, string channel, BtParam<bool> value)
    {
        Id = id;
        _channel = channel;
        _value = value;
    }

    public string Id { get; }
    public string Type => "IO.SetOutput";
    private readonly string _channel;
    private readonly BtParam<bool> _value;

    public Task<NodeStatus> TickAsync(BtContext context)
    {
        if (context.Io is null)
            throw new InvalidOperationException("IO adapter is not configured.");

        context.Io.SetOutput(_channel, _value.Resolve(context));
        return Task.FromResult(NodeStatus.Success);
    }
}

internal sealed class WaitInputNode : INode
{
    private sealed class State
    {
        public DateTimeOffset? Start;
    }

    public WaitInputNode(string id, string channel, BtParam<bool> expected, TimeSpan? timeout)
    {
        Id = id;
        _channel = channel;
        _expected = expected;
        _timeout = timeout;
    }

    public string Id { get; }
    public string Type => "IO.WaitInput";
    private readonly string _channel;
    private readonly BtParam<bool> _expected;
    private readonly TimeSpan? _timeout;

    public async Task<NodeStatus> TickAsync(BtContext context)
    {
        if (context.Io is null)
            throw new InvalidOperationException("IO adapter is not configured.");

        var state = context.GetOrCreateState(Id, () => new State());
        state.Start ??= context.Clock.UtcNow;

        if (_timeout is not null && context.Clock.UtcNow - state.Start.Value > _timeout.Value)
        {
            state.Start = null;
            return NodeStatus.Failure;
        }

        var current = context.Io.GetInput(_channel);
        if (current == _expected.Resolve(context))
        {
            state.Start = null;
            return NodeStatus.Success;
        }

        await context.Clock.Delay(TimeSpan.FromMilliseconds(5), context.CancellationToken).ConfigureAwait(false);
        return NodeStatus.Running;
    }
}

