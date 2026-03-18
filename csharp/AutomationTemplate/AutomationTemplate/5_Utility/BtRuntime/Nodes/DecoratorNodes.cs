using System;
using System.Threading.Tasks;

namespace AutomationTemplate._5_Utility.BtRuntime.Nodes
{
    internal sealed class TimeoutDecoratorNode : INode
    {
        private sealed class State { public DateTime? StartUtc; }

        public TimeoutDecoratorNode(string id, INode child, TimeSpan timeout)
        {
            Id = id;
            this.child = child;
            this.timeout = timeout;
        }

        public string Id { get; }
        public string Type => "Decorator.Timeout";
        private readonly INode child;
        private readonly TimeSpan timeout;

        public async Task<BtNodeStatus> TickAsync(BtContext context)
        {
            var state = context.GetOrCreateState(Id, () => new State());
            state.StartUtc = state.StartUtc ?? DateTime.UtcNow;

            if (DateTime.UtcNow - state.StartUtc.Value > timeout)
            {
                state.StartUtc = null;
                context.Emit(Id, Type, "timeout", BtNodeStatus.Failure);
                return BtNodeStatus.Failure;
            }

            var status = await child.TickAsync(context).ConfigureAwait(false);
            if (status != BtNodeStatus.Running) state.StartUtc = null;
            return status;
        }
    }
}

