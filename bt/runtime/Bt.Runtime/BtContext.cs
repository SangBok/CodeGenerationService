using System.Collections.Concurrent;
using System.Text.Json;

namespace Bt.Runtime;

public sealed class BtContext
{
    public BtContext(
        BtBlackboard blackboard,
        CancellationToken cancellationToken,
        Action<TraceEvent>? trace = null,
        IUnitRegistry? units = null,
        IIoAdapter? io = null,
        IClock? clock = null)
    {
        Blackboard = blackboard;
        CancellationToken = cancellationToken;
        Trace = trace;
        Units = units;
        Io = io;
        Clock = clock ?? SystemClock.Instance;
    }

    public BtBlackboard Blackboard { get; }
    public CancellationToken CancellationToken { get; }
    public Action<TraceEvent>? Trace { get; }

    public IUnitRegistry? Units { get; }
    public IIoAdapter? Io { get; }
    public IClock Clock { get; }

    internal ConcurrentDictionary<string, object> NodeState { get; } = new(StringComparer.Ordinal);

    internal TState GetOrCreateState<TState>(string nodeId, Func<TState> factory) where TState : class
    {
        return (TState)NodeState.GetOrAdd(nodeId, _ => factory());
    }
}

public interface IClock
{
    DateTimeOffset UtcNow { get; }
    Task Delay(TimeSpan delay, CancellationToken cancellationToken);
}

public sealed class SystemClock : IClock
{
    public static readonly SystemClock Instance = new();
    private SystemClock() { }

    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

    public Task Delay(TimeSpan delay, CancellationToken cancellationToken) =>
        Task.Delay(delay, cancellationToken);
}

public interface IIoAdapter
{
    void SetOutput(string channel, bool value);
    bool GetInput(string channel);
}

public interface IUnitRegistry
{
    object GetUnit(string unitName);
}

