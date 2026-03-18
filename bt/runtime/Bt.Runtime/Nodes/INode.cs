namespace Bt.Runtime.Nodes;

public interface INode
{
    string Id { get; }
    string Type { get; }

    Task<NodeStatus> TickAsync(BtContext context);
}

