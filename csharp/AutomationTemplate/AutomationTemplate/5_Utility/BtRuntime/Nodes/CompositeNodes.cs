using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutomationTemplate._5_Utility.BtRuntime.Nodes
{
    internal sealed class SequenceNode : INode
    {
        private sealed class State { public int Index; }

        public SequenceNode(string id, List<INode> children)
        {
            Id = id;
            this.children = children ?? new List<INode>();
        }

        public string Id { get; }
        public string Type => "Sequence";
        private readonly List<INode> children;

        public async Task<BtNodeStatus> TickAsync(BtContext context)
        {
            var state = context.GetOrCreateState(Id, () => new State());

            for (; state.Index < children.Count; state.Index++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                var status = await children[state.Index].TickAsync(context).ConfigureAwait(false);
                if (status == BtNodeStatus.Running) return BtNodeStatus.Running;
                if (status == BtNodeStatus.Failure)
                {
                    state.Index = 0;
                    return BtNodeStatus.Failure;
                }
            }

            state.Index = 0;
            return BtNodeStatus.Success;
        }
    }

    internal sealed class SelectorNode : INode
    {
        private sealed class State { public int Index; }

        public SelectorNode(string id, List<INode> children)
        {
            Id = id;
            this.children = children ?? new List<INode>();
        }

        public string Id { get; }
        public string Type => "Selector";
        private readonly List<INode> children;

        public async Task<BtNodeStatus> TickAsync(BtContext context)
        {
            var state = context.GetOrCreateState(Id, () => new State());

            for (; state.Index < children.Count; state.Index++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                var status = await children[state.Index].TickAsync(context).ConfigureAwait(false);
                if (status == BtNodeStatus.Running) return BtNodeStatus.Running;
                if (status == BtNodeStatus.Success)
                {
                    state.Index = 0;
                    return BtNodeStatus.Success;
                }
            }

            state.Index = 0;
            return BtNodeStatus.Failure;
        }
    }
}

