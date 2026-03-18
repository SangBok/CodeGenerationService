using Bt.Runtime;
using Bt.Runtime.Nodes;

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

var trace = new List<TraceEvent>();
void Trace(TraceEvent ev)
{
    trace.Add(ev);
    if (ev.EventType is "tickResult" or "timeout")
        Console.WriteLine($"[{ev.Timestamp:O}] {ev.EventType} node={ev.NodeId} type={ev.NodeType} status={ev.Status} msg={ev.Message}");
}

var inputPath = args.Length > 0 ? args[0] : Path.Combine(AppContext.BaseDirectory, "move-safe-pos.tree.json");
if (!File.Exists(inputPath))
    throw new FileNotFoundException($"BT definition file not found: {inputPath}");

var def = BtDefinition.LoadFromFile(inputPath);
var tree = BehaviorTree.Build(def);

var blackboard = new BtBlackboard();
blackboard.Set("positionIdx", 2);

var units = new DemoUnitRegistry(new DemoStageUnit());
var context = new BtContext(
    blackboard,
    cts.Token,
    trace: Trace,
    units: units);

Console.WriteLine($"Running tree '{def.TreeId}' v{def.Version} (Ctrl+C to cancel)...");
var status = await tree.RunToCompletionAsync(context, tickInterval: TimeSpan.FromMilliseconds(10));
Console.WriteLine($"Done: {status}");

sealed class DemoUnitRegistry : IUnitRegistry
{
    private readonly IStageUnit _stage;
    public DemoUnitRegistry(IStageUnit stage) => _stage = stage;
    public object GetUnit(string unitName) => _stage;
}

sealed class DemoStageUnit : IStageUnit
{
    public void MoveAxisAbsolute(UnitAxis axis, int positionIdx) =>
        Console.WriteLine($"[Unit] MoveAxisAbsolute axis={axis} positionIdx={positionIdx}");

    public void MoveXYToPosition(int positionIdx) =>
        Console.WriteLine($"[Unit] MoveXYToPosition positionIdx={positionIdx}");

    public void CylinderForward(string cylinderName) =>
        Console.WriteLine($"[Unit] CylinderForward name={cylinderName}");

    public void CylinderBackward(string cylinderName) =>
        Console.WriteLine($"[Unit] CylinderBackward name={cylinderName}");
}
