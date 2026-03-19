using System;

namespace AutomationTemplate._1_Hardware
{
    public sealed class AxisMoveEventArgs : EventArgs
    {
        public AxisMoveEventArgs(string axisId, double position)
        {
            AxisId = axisId;
            Position = position;
        }

        public string AxisId { get; }
        public double Position { get; }
    }

    public interface IAxisMoveNotifier
    {
        event EventHandler<AxisMoveEventArgs> MoveAbsoluteStarted;
        event EventHandler<AxisMoveEventArgs> MoveAbsoluteCompleted;
    }
}

