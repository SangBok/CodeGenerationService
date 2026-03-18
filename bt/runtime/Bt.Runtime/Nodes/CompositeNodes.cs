namespace Bt.Runtime.Nodes;

internal sealed class SequenceNode : INode
{
    private sealed class State
    {
        public int Index;
    }

    public SequenceNode(string id, IReadOnlyList<INode> children)
    {
        Id = id;
        _children = children;
    }

    public string Id { get; }
    public string Type => "Sequence";
    private readonly IReadOnlyList<INode> _children;

    public async Task<NodeStatus> TickAsync(BtContext context)
    {
        var state = context.GetOrCreateState(Id, () => new State());

        for (; state.Index < _children.Count; state.Index++)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            var child = _children[state.Index];
            var status = await child.TickAsync(context).ConfigureAwait(false);

            if (status == NodeStatus.Running) return NodeStatus.Running;
            if (status == NodeStatus.Failure)
            {
                state.Index = 0;
                return NodeStatus.Failure;
            }
        }

        state.Index = 0;
        return NodeStatus.Success;
    }
}

internal sealed class SelectorNode : INode
{
    private sealed class State
    {
        public int Index;
    }

    public SelectorNode(string id, IReadOnlyList<INode> children)
    {
        Id = id;
        _children = children;
    }

    public string Id { get; }
    public string Type => "Selector";
    private readonly IReadOnlyList<INode> _children;

    public async Task<NodeStatus> TickAsync(BtContext context)
    {
        var state = context.GetOrCreateState(Id, () => new State());

        for (; state.Index < _children.Count; state.Index++)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            var child = _children[state.Index];
            var status = await child.TickAsync(context).ConfigureAwait(false);

            if (status == NodeStatus.Running) return NodeStatus.Running;
            if (status == NodeStatus.Success)
            {
                state.Index = 0;
                return NodeStatus.Success;
            }
        }

        state.Index = 0;
        return NodeStatus.Failure;
    }
}

