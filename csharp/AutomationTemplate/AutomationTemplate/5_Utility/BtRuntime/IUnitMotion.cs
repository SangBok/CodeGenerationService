namespace AutomationTemplate._5_Utility.BtRuntime
{
    public interface IUnitMotion
    {
        int MovePositionZ(int positionidx);
        int MovePositionXY(int positionidx);
        int MaterialGrip();
        int MaterialUngrip();
    }
}
