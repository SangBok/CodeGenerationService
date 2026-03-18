using System.Text.Json;
using Bt.Runtime.Nodes;

namespace Bt.Runtime;

public sealed class BehaviorTree
{
    private BehaviorTree(BtDefinition definition, INode root, IReadOnlyDictionary<string, INode> nodes)
    {
        Definition = definition;
        Root = root;
        Nodes = nodes;
    }

    public BtDefinition Definition { get; }
    public INode Root { get; }
    public IReadOnlyDictionary<string, INode> Nodes { get; }

    public static BehaviorTree Build(BtDefinition definition)
    {
        var byId = definition.Nodes.ToDictionary(n => n.Id, StringComparer.Ordinal);
        if (!byId.ContainsKey(definition.RootNodeId))
            throw new InvalidOperationException($"RootNodeId '{definition.RootNodeId}' does not exist.");

        var built = new Dictionary<string, INode>(StringComparer.Ordinal);
        INode BuildNode(string id)
        {
            if (built.TryGetValue(id, out var existing)) return existing;
            if (!byId.TryGetValue(id, out var nodeDef))
                throw new InvalidOperationException($"Node '{id}' not found.");

            var node = NodeFactory.Create(nodeDef, BuildNode);
            built[id] = node;
            return node;
        }

        var root = BuildNode(definition.RootNodeId);
        return new BehaviorTree(definition, root, built);
    }

    public async Task<NodeStatus> RunToCompletionAsync(BtContext context, TimeSpan tickInterval)
    {
        context.Blackboard.SeedFromBlackboardSchema(Definition.BlackboardSchema);

        while (true)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var status = await TickOnceAsync(context).ConfigureAwait(false);
            if (status != NodeStatus.Running) return status;

            await context.Clock.Delay(tickInterval, context.CancellationToken).ConfigureAwait(false);
        }
    }

    public async Task<NodeStatus> TickOnceAsync(BtContext context)
    {
        context.Trace?.Invoke(new TraceEvent(context.Clock.UtcNow, Root.Id, Root.Type, "tick"));
        var status = await Root.TickAsync(context).ConfigureAwait(false);
        context.Trace?.Invoke(new TraceEvent(context.Clock.UtcNow, Root.Id, Root.Type, "tickResult", status));
        return status;
    }
}

internal static class NodeFactory
{
    public static INode Create(BtDefinition.Node def, Func<string, INode> getNode)
    {
        def.Children ??= new List<string>();
        def.Parameters ??= new Dictionary<string, JsonElement>(StringComparer.Ordinal);

        return def.Type switch
        {
            "Sequence" => new SequenceNode(def.Id, def.Children.Select(getNode).ToList()),
            "Selector" => new SelectorNode(def.Id, def.Children.Select(getNode).ToList()),

            "Decorator.Timeout" => new TimeoutDecoratorNode(
                def.Id,
                def.Children.Count == 1 ? getNode(def.Children[0]) : throw new InvalidOperationException("Timeout requires exactly 1 child."),
                timeout: TimeSpan.FromMilliseconds(GetInt(def, "timeoutMs"))),

            "Action.Delay" => new DelayNode(def.Id, TimeSpan.FromMilliseconds(GetInt(def, "delayMs"))),
            "Condition.AlwaysTrue" => new AlwaysTrueNode(def.Id),
            "Condition.AlwaysFalse" => new AlwaysFalseNode(def.Id),

            "IO.SetOutput" => new SetOutputNode(def.Id, GetString(def, "channel"), GetBoolParam(def, "value")),
            "IO.WaitInput" => new WaitInputNode(
                def.Id,
                GetString(def, "channel"),
                GetBoolParam(def, "expected"),
                def.Parameters.ContainsKey("timeoutMs") ? TimeSpan.FromMilliseconds(GetInt(def, "timeoutMs")) : null),

            "Unit.AxisMoveAbsolute" => new AxisMoveAbsoluteNode(
                def.Id,
                GetString(def, "unit"),
                Enum.Parse<UnitAxis>(GetString(def, "axis"), ignoreCase: true),
                GetIntParam(def, "positionIdx")),

            "Unit.MoveXYToPosition" => new MoveXYToPositionNode(
                def.Id,
                GetString(def, "unit"),
                GetIntParam(def, "positionIdx")),

            "Unit.CylinderForward" => new CylinderForwardNode(def.Id, GetString(def, "unit"), GetString(def, "cylinder")),
            "Unit.CylinderBackward" => new CylinderBackwardNode(def.Id, GetString(def, "unit"), GetString(def, "cylinder")),

            _ => throw new NotSupportedException($"Unknown node type '{def.Type}'.")
        };
    }

    private static int GetInt(BtDefinition.Node def, string param) => Convert.ToInt32(BtValueResolver.ToDotNetObject(GetElement(def, param).ValueKind, GetElement(def, param)));
    private static string GetString(BtDefinition.Node def, string param) => Convert.ToString(BtValueResolver.ToDotNetObject(GetElement(def, param).ValueKind, GetElement(def, param))) ?? "";

    private static BtParam<int> GetIntParam(BtDefinition.Node def, string param) =>
        new(param, GetElement(def, param), raw => Convert.ToInt32(raw));

    private static BtParam<bool> GetBoolParam(BtDefinition.Node def, string param) =>
        new(param, GetElement(def, param), raw => Convert.ToBoolean(raw));

    private static JsonElement GetElement(BtDefinition.Node def, string param)
    {
        if (!def.Parameters!.TryGetValue(param, out var element))
            throw new InvalidOperationException($"Node '{def.Id}' is missing required parameter '{param}'.");
        return element;
    }
}

