using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AutomationTemplate._5_Utility.BtRuntime.Nodes
{
    internal sealed class UnitAxisMoveAbsoluteNode : INode
    {
        public UnitAxisMoveAbsoluteNode(string id, string name, string unitName, string axis, JToken positionIdxToken)
        {
            Id = id;
            Name = name;
            this.unitName = unitName;
            this.axis = axis;
            this.positionIdxToken = positionIdxToken;
        }

        public string Id { get; }
        public string Name { get; }
        public string Type => "Unit.AxisMoveAbsolute";
        private readonly string unitName;
        private readonly string axis;
        private readonly JToken positionIdxToken;

        public Task<BtNodeStatus> TickAsync(BtContext context)
        {
            context.Emit(Id, Name, Type, "tickStart");
            var unitObj = context.UnitRegistry.ResolveUnit(unitName);
            var unitMotion = unitObj as IUnitMotion;
            if (unitMotion == null)
                throw new InvalidOperationException("Unit '" + unitName + "' is not IUnitMotion.");

            var positionIdx = BtJson.ResolveInt(positionIdxToken, context.Blackboard);

            if (string.Equals(axis, "Z", StringComparison.OrdinalIgnoreCase))
            {
                unitMotion.MovePositionZ(positionIdx);
                context.Emit(Id, Name, Type, "tickResult", BtNodeStatus.Success);
                return Task.FromResult(BtNodeStatus.Success);
            }

            throw new NotSupportedException("Legacy handler axis move supports only axis=Z. Use Unit.MoveXYToPosition for XY.");
        }
    }

    internal sealed class UnitMoveXYToPositionNode : INode
    {
        public UnitMoveXYToPositionNode(string id, string name, string unitName, JToken positionIdxToken)
        {
            Id = id;
            Name = name;
            this.unitName = unitName;
            this.positionIdxToken = positionIdxToken;
        }

        public string Id { get; }
        public string Name { get; }
        public string Type => "Unit.MoveXYToPosition";
        private readonly string unitName;
        private readonly JToken positionIdxToken;

        public Task<BtNodeStatus> TickAsync(BtContext context)
        {
            context.Emit(Id, Name, Type, "tickStart");
            var unitObj = context.UnitRegistry.ResolveUnit(unitName);
            var unitMotion = unitObj as IUnitMotion;
            if (unitMotion == null)
                throw new InvalidOperationException("Unit '" + unitName + "' is not IUnitMotion.");

            var positionIdx = BtJson.ResolveInt(positionIdxToken, context.Blackboard);
            unitMotion.MovePositionXY(positionIdx);
            context.Emit(Id, Name, Type, "tickResult", BtNodeStatus.Success);
            return Task.FromResult(BtNodeStatus.Success);
        }
    }

    internal sealed class UnitCylinderForwardNode : INode
    {
        public UnitCylinderForwardNode(string id, string name, string unitName, JToken cylinderToken)
        {
            Id = id;
            Name = name;
            this.unitName = unitName;
            this.cylinderToken = cylinderToken;
        }

        public string Id { get; }
        public string Name { get; }
        public string Type => "Unit.CylinderForward";

        private readonly string unitName;
        private readonly JToken cylinderToken;

        public Task<BtNodeStatus> TickAsync(BtContext context)
        {
            context.Emit(Id, Name, Type, "tickStart");

            var unitObj = context.UnitRegistry.ResolveUnit(unitName);
            var unitMotion = unitObj as IUnitMotion;
            if (unitMotion == null)
                throw new InvalidOperationException("Unit '" + unitName + "' is not IUnitMotion.");

            var cylinderName = BtJson.ResolveString(cylinderToken, context.Blackboard);
            if (!string.Equals(cylinderName, "Gripper", StringComparison.OrdinalIgnoreCase))
                throw new NotSupportedException("Legacy handler cylinder forward supports only cylinder=Gripper.");

            unitMotion.MaterialGrip();
            context.Emit(Id, Name, Type, "tickResult", BtNodeStatus.Success);
            return Task.FromResult(BtNodeStatus.Success);
        }
    }

    internal sealed class UnitCylinderBackwardNode : INode
    {
        public UnitCylinderBackwardNode(string id, string name, string unitName, JToken cylinderToken)
        {
            Id = id;
            Name = name;
            this.unitName = unitName;
            this.cylinderToken = cylinderToken;
        }

        public string Id { get; }
        public string Name { get; }
        public string Type => "Unit.CylinderBackward";

        private readonly string unitName;
        private readonly JToken cylinderToken;

        public Task<BtNodeStatus> TickAsync(BtContext context)
        {
            context.Emit(Id, Name, Type, "tickStart");

            var unitObj = context.UnitRegistry.ResolveUnit(unitName);
            var unitMotion = unitObj as IUnitMotion;
            if (unitMotion == null)
                throw new InvalidOperationException("Unit '" + unitName + "' is not IUnitMotion.");

            var cylinderName = BtJson.ResolveString(cylinderToken, context.Blackboard);
            if (!string.Equals(cylinderName, "Gripper", StringComparison.OrdinalIgnoreCase))
                throw new NotSupportedException("Legacy handler cylinder backward supports only cylinder=Gripper.");

            unitMotion.MaterialUngrip();
            context.Emit(Id, Name, Type, "tickResult", BtNodeStatus.Success);
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

        public static string ResolveString(JToken token, BtBlackboard blackboard)
        {
            if (token == null) throw new InvalidOperationException("Missing string token.");

            if (token.Type == JTokenType.Object && token["$ref"] != null)
            {
                var key = token["$ref"].Value<string>();
                var raw = blackboard.GetRequired(key);
                return Convert.ToString(raw);
            }

            return token.Value<string>();
        }
    }
}

