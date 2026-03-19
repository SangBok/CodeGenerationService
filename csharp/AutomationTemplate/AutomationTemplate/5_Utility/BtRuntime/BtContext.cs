using System;
using System.Collections.Concurrent;
using System.Threading;

namespace AutomationTemplate._5_Utility.BtRuntime
{
    public sealed class BtContext
    {
        public BtContext(BtBlackboard blackboard, IUnitRegistry unitRegistry, CancellationToken cancellationToken)
        {
            Blackboard = blackboard;
            UnitRegistry = unitRegistry;
            CancellationToken = cancellationToken;
        }

        public BtBlackboard Blackboard { get; }
        public IUnitRegistry UnitRegistry { get; }
        public CancellationToken CancellationToken { get; }

        public event EventHandler<BtTraceEventArgs> Trace;

        internal ConcurrentDictionary<string, object> NodeState { get; } =
            new ConcurrentDictionary<string, object>(StringComparer.Ordinal);

        internal TState GetOrCreateState<TState>(string nodeId, Func<TState> factory) where TState : class
        {
            return (TState)NodeState.GetOrAdd(nodeId, _ => factory());
        }

        internal void Emit(string nodeId, string nodeName, string nodeType, string eventType, BtNodeStatus? status = null, string message = "")
        {
            Trace?.Invoke(this, new BtTraceEventArgs(DateTime.UtcNow, nodeId, nodeName, nodeType, eventType, status, message ?? ""));
        }
    }

    public interface IUnitRegistry
    {
        object ResolveUnit(string unitName);
    }
}

