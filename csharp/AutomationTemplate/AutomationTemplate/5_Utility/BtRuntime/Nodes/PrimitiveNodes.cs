using System;
using System.Threading.Tasks;

namespace AutomationTemplate._5_Utility.BtRuntime.Nodes
{
    internal sealed class DelayNode : INode
    {
        private sealed class State
        {
            public DateTime? FinishUtc;
        }

        public DelayNode(string id, TimeSpan delay)
        {
            Id = id;
            this.delay = delay;
        }

        public string Id { get; }
        public string Type => "Action.Delay";
        private readonly TimeSpan delay;

        public Task<BtNodeStatus> TickAsync(BtContext context)
        {
            var state = context.GetOrCreateState(Id, () => new State());
            state.FinishUtc = state.FinishUtc ?? (DateTime.UtcNow + delay);

            if (DateTime.UtcNow >= state.FinishUtc.Value)
            {
                state.FinishUtc = null;
                return Task.FromResult(BtNodeStatus.Success);
            }

            return Task.FromResult(BtNodeStatus.Running);
        }
    }

    internal sealed class AlwaysTrueNode : INode
    {
        public AlwaysTrueNode(string id) { Id = id; }
        public string Id { get; }
        public string Type => "Condition.AlwaysTrue";
        public Task<BtNodeStatus> TickAsync(BtContext context) => Task.FromResult(BtNodeStatus.Success);
    }

    internal sealed class AlwaysFalseNode : INode
    {
        public AlwaysFalseNode(string id) { Id = id; }
        public string Id { get; }
        public string Type => "Condition.AlwaysFalse";
        public Task<BtNodeStatus> TickAsync(BtContext context) => Task.FromResult(BtNodeStatus.Failure);
    }
}

