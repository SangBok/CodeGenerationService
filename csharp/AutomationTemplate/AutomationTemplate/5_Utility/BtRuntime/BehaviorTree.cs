using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutomationTemplate._5_Utility.BtRuntime.Nodes;
using Newtonsoft.Json.Linq;

namespace AutomationTemplate._5_Utility.BtRuntime
{
    public sealed class BehaviorTree
    {
        private BehaviorTree(BtDefinition definition, INode root)
        {
            Definition = definition;
            Root = root;
        }

        public BtDefinition Definition { get; }
        private INode Root { get; }

        public static BehaviorTree Build(BtDefinition definition)
        {
            if (definition == null) throw new ArgumentNullException(nameof(definition));
            if (definition.Nodes == null || definition.Nodes.Count == 0) throw new InvalidOperationException("BT nodes are empty.");

            var byId = definition.Nodes.ToDictionary(n => n.Id, StringComparer.Ordinal);
            if (!byId.ContainsKey(definition.RootNodeId))
                throw new InvalidOperationException("RootNodeId not found: " + definition.RootNodeId);

            var built = new Dictionary<string, INode>(StringComparer.Ordinal);

            INode BuildNode(string id)
            {
                if (built.TryGetValue(id, out var existing)) return existing;
                if (!byId.TryGetValue(id, out var def)) throw new InvalidOperationException("Node not found: " + id);

                def.Children = def.Children ?? new List<string>();
                def.Parameters = def.Parameters ?? new JObject();

                INode node;
                switch (def.Type)
                {
                    case "Sequence":
                        node = new SequenceNode(def.Id, def.Name ?? def.Id, def.Children.Select(BuildNode).ToList());
                        break;
                    case "Selector":
                        node = new SelectorNode(def.Id, def.Name ?? def.Id, def.Children.Select(BuildNode).ToList());
                        break;
                    case "Decorator.Timeout":
                        node = new TimeoutDecoratorNode(
                            def.Id,
                            def.Name ?? def.Id,
                            def.Children.Count == 1 ? BuildNode(def.Children[0]) : throw new InvalidOperationException("Timeout requires exactly 1 child."),
                            TimeSpan.FromMilliseconds(GetRequiredInt(def, "timeoutMs")));
                        break;
                    case "Action.Delay":
                        node = new DelayNode(def.Id, def.Name ?? def.Id, TimeSpan.FromMilliseconds(GetRequiredInt(def, "delayMs")));
                        break;
                    case "Condition.AlwaysTrue":
                        node = new AlwaysTrueNode(def.Id, def.Name ?? def.Id);
                        break;
                    case "Condition.AlwaysFalse":
                        node = new AlwaysFalseNode(def.Id, def.Name ?? def.Id);
                        break;

                    // Legacy handler unit actions (MHandler)
                    case "Unit.AxisMoveAbsolute":
                        node = new HandlerAxisMoveAbsoluteNode(
                            def.Id,
                            def.Name ?? def.Id,
                            GetRequiredString(def, "unit"),
                            GetRequiredString(def, "axis"),
                            GetRequiredToken(def, "positionIdx"));
                        break;
                    case "Unit.MoveXYToPosition":
                        node = new HandlerMoveXYToPositionNode(
                            def.Id,
                            def.Name ?? def.Id,
                            GetRequiredString(def, "unit"),
                            GetRequiredToken(def, "positionIdx"));
                        break;
                    case "Unit.CylinderForward":
                        node = new HandlerCylinderForwardNode(
                            def.Id,
                            def.Name ?? def.Id,
                            GetRequiredString(def, "unit"),
                            GetRequiredToken(def, "cylinder"));
                        break;
                    case "Unit.CylinderBackward":
                        node = new HandlerCylinderBackwardNode(
                            def.Id,
                            def.Name ?? def.Id,
                            GetRequiredString(def, "unit"),
                            GetRequiredToken(def, "cylinder"));
                        break;

                    default:
                        throw new NotSupportedException("Unknown node type: " + def.Type);
                }

                built[id] = node;
                return node;
            }

            var root = BuildNode(definition.RootNodeId);
            return new BehaviorTree(definition, root);
        }

        public async Task<BtNodeStatus> RunToCompletionAsync(BtContext context, TimeSpan tickInterval)
        {
            context.Blackboard.SeedDefaultsFromSchema(Definition);

            while (true)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                var status = await TickOnceAsync(context).ConfigureAwait(false);
                if (status != BtNodeStatus.Running) return status;

                await Task.Delay(tickInterval, context.CancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<BtNodeStatus> TickOnceAsync(BtContext context)
        {
            return await Root.TickAsync(context).ConfigureAwait(false);
        }

        private static string GetRequiredString(BtDefinition.Node node, string name)
        {
            var t = GetRequiredToken(node, name);
            return t.Type == JTokenType.String ? t.Value<string>() : t.ToString();
        }

        private static int GetRequiredInt(BtDefinition.Node node, string name)
        {
            var t = GetRequiredToken(node, name);
            return t.Value<int>();
        }

        private static JToken GetRequiredToken(BtDefinition.Node node, string name)
        {
            if (node.Parameters == null || node.Parameters[name] == null)
                throw new InvalidOperationException("Node '" + node.Id + "' is missing parameter '" + name + "'.");
            return node.Parameters[name];
        }
    }
}

