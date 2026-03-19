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

        public DelayNode(string id, string name, TimeSpan delay)
        {
            Id = id;
            Name = name;
            this.delay = delay;
        }

        public string Id { get; }
        public string Name { get; }
        public string Type => "Action.Delay";
        private readonly TimeSpan delay;

        public Task<BtNodeStatus> TickAsync(BtContext context)
        {
            context.Emit(Id, Name, Type, "tickStart");
            var state = context.GetOrCreateState(Id, () => new State());
            state.FinishUtc = state.FinishUtc ?? (DateTime.UtcNow + delay);

            if (DateTime.UtcNow >= state.FinishUtc.Value)
            {
                state.FinishUtc = null;
                context.Emit(Id, Name, Type, "tickResult", BtNodeStatus.Success);
                return Task.FromResult(BtNodeStatus.Success);
            }

            context.Emit(Id, Name, Type, "tickResult", BtNodeStatus.Running);
            return Task.FromResult(BtNodeStatus.Running);
        }
    }

    internal sealed class AlwaysTrueNode : INode
    {
        public AlwaysTrueNode(string id, string name)
        {
            Id = id;
            Name = name;
        }
        public string Id { get; }
        public string Name { get; }
        public string Type => "Condition.AlwaysTrue";
        public Task<BtNodeStatus> TickAsync(BtContext context)
        {
            context.Emit(Id, Name, Type, "tickStart");
            context.Emit(Id, Name, Type, "tickResult", BtNodeStatus.Success);
            return Task.FromResult(BtNodeStatus.Success);
        }
    }

    internal sealed class AlwaysFalseNode : INode
    {
        public AlwaysFalseNode(string id, string name)
        {
            Id = id;
            Name = name;
        }
        public string Id { get; }
        public string Name { get; }
        public string Type => "Condition.AlwaysFalse";
        public Task<BtNodeStatus> TickAsync(BtContext context)
        {
            context.Emit(Id, Name, Type, "tickStart");
            context.Emit(Id, Name, Type, "tickResult", BtNodeStatus.Failure);
            return Task.FromResult(BtNodeStatus.Failure);
        }
    }
}

