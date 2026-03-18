using System;
using System.Threading.Tasks;
using AutomationTemplate._2_Mechanical;
using Newtonsoft.Json.Linq;

namespace AutomationTemplate._5_Utility.BtRuntime.Nodes
{
    internal sealed class HandlerAxisMoveAbsoluteNode : INode
    {
        public HandlerAxisMoveAbsoluteNode(string id, string unitName, string axis, JToken positionIdxToken)
        {
            Id = id;
            this.unitName = unitName;
            this.axis = axis;
            this.positionIdxToken = positionIdxToken;
        }

        public string Id { get; }
        public string Type => "Unit.AxisMoveAbsolute";
        private readonly string unitName;
        private readonly string axis;
        private readonly JToken positionIdxToken;

        public Task<BtNodeStatus> TickAsync(BtContext context)
        {
            var unitObj = context.UnitRegistry.ResolveUnit(unitName);
            var handler = unitObj as MHandler;
            if (handler == null)
                throw new InvalidOperationException("Unit '" + unitName + "' is not MHandler (legacy handler).");

            var positionIdx = BtJson.ResolveInt(positionIdxToken, context.Blackboard);

            // Legacy MHandler supports MovePositionZ and MovePositionXY
            if (string.Equals(axis, "Z", StringComparison.OrdinalIgnoreCase))
            {
                handler.MovePositionZ(positionIdx);
                return Task.FromResult(BtNodeStatus.Success);
            }

            throw new NotSupportedException("Legacy handler axis move supports only axis=Z. Use Unit.MoveXYToPosition for XY.");
        }
    }

    internal sealed class HandlerMoveXYToPositionNode : INode
    {
        public HandlerMoveXYToPositionNode(string id, string unitName, JToken positionIdxToken)
        {
            Id = id;
            this.unitName = unitName;
            this.positionIdxToken = positionIdxToken;
        }

        public string Id { get; }
        public string Type => "Unit.MoveXYToPosition";
        private readonly string unitName;
        private readonly JToken positionIdxToken;

        public Task<BtNodeStatus> TickAsync(BtContext context)
        {
            var unitObj = context.UnitRegistry.ResolveUnit(unitName);
            var handler = unitObj as MHandler;
            if (handler == null)
                throw new InvalidOperationException("Unit '" + unitName + "' is not MHandler (legacy handler).");

            var positionIdx = BtJson.ResolveInt(positionIdxToken, context.Blackboard);
            handler.MovePositionXY(positionIdx);
            return Task.FromResult(BtNodeStatus.Success);
        }
    }

    internal static class BtJson
    {
        public static int ResolveInt(JToken token, BtBlackboard blackboard)
        {
            if (token == null) throw new InvalidOperationException("Missing int token.");

            if (token.Type == JTokenType.Object && token["$ref"] != null)
            {
                var key = token["$ref"].Value<string>();
                var raw = blackboard.GetRequired(key);
                return Convert.ToInt32(raw);
            }

            return token.Value<int>();
        }
    }
}

