namespace Bt.Runtime.Nodes;

public enum UnitAxis
{
    X,
    Y,
    Z
}

public interface IStageUnit
{
    void MoveAxisAbsolute(UnitAxis axis, int positionIdx);
    void MoveXYToPosition(int positionIdx);
    void CylinderForward(string cylinderName);
    void CylinderBackward(string cylinderName);
}

internal sealed class AxisMoveAbsoluteNode : INode
{
    public AxisMoveAbsoluteNode(string id, string unitName, UnitAxis axis, BtParam<int> positionIdx)
    {
        Id = id;
        _unitName = unitName;
        _axis = axis;
        _positionIdx = positionIdx;
    }

    public string Id { get; }
    public string Type => "Unit.AxisMoveAbsolute";
    private readonly string _unitName;
    private readonly UnitAxis _axis;
    private readonly BtParam<int> _positionIdx;

    public Task<NodeStatus> TickAsync(BtContext context)
    {
        if (context.Units is null)
            throw new InvalidOperationException("Unit registry is not configured.");

        var unitObj = context.Units.GetUnit(_unitName);
        if (unitObj is not IStageUnit unit)
            throw new InvalidOperationException($"Unit '{_unitName}' is not an IStageUnit.");

        unit.MoveAxisAbsolute(_axis, _positionIdx.Resolve(context));
        return Task.FromResult(NodeStatus.Success);
    }
}

internal sealed class MoveXYToPositionNode : INode
{
    public MoveXYToPositionNode(string id, string unitName, BtParam<int> positionIdx)
    {
        Id = id;
        _unitName = unitName;
        _positionIdx = positionIdx;
    }

    public string Id { get; }
    public string Type => "Unit.MoveXYToPosition";
    private readonly string _unitName;
    private readonly BtParam<int> _positionIdx;

    public Task<NodeStatus> TickAsync(BtContext context)
    {
        if (context.Units is null)
            throw new InvalidOperationException("Unit registry is not configured.");

        var unitObj = context.Units.GetUnit(_unitName);
        if (unitObj is not IStageUnit unit)
            throw new InvalidOperationException($"Unit '{_unitName}' is not an IStageUnit.");

        unit.MoveXYToPosition(_positionIdx.Resolve(context));
        return Task.FromResult(NodeStatus.Success);
    }
}

internal sealed class CylinderForwardNode : INode
{
    public CylinderForwardNode(string id, string unitName, string cylinderName)
    {
        Id = id;
        _unitName = unitName;
        _cylinderName = cylinderName;
    }

    public string Id { get; }
    public string Type => "Unit.CylinderForward";
    private readonly string _unitName;
    private readonly string _cylinderName;

    public Task<NodeStatus> TickAsync(BtContext context)
    {
        if (context.Units is null)
            throw new InvalidOperationException("Unit registry is not configured.");

        var unitObj = context.Units.GetUnit(_unitName);
        if (unitObj is not IStageUnit unit)
            throw new InvalidOperationException($"Unit '{_unitName}' is not an IStageUnit.");

        unit.CylinderForward(_cylinderName);
        return Task.FromResult(NodeStatus.Success);
    }
}

internal sealed class CylinderBackwardNode : INode
{
    public CylinderBackwardNode(string id, string unitName, string cylinderName)
    {
        Id = id;
        _unitName = unitName;
        _cylinderName = cylinderName;
    }

    public string Id { get; }
    public string Type => "Unit.CylinderBackward";
    private readonly string _unitName;
    private readonly string _cylinderName;

    public Task<NodeStatus> TickAsync(BtContext context)
    {
        if (context.Units is null)
            throw new InvalidOperationException("Unit registry is not configured.");

        var unitObj = context.Units.GetUnit(_unitName);
        if (unitObj is not IStageUnit unit)
            throw new InvalidOperationException($"Unit '{_unitName}' is not an IStageUnit.");

        unit.CylinderBackward(_cylinderName);
        return Task.FromResult(NodeStatus.Success);
    }
}

