using System;

namespace AutomationTemplate._1_Hardware.Mock
{
    public sealed class MockAxisController : HIAxisController
    {
        private readonly object gate = new object();
        private bool servoOn;
        private bool movingHome;
        private double actualPosition;

        public MockAxisController(string axisId)
        {
            if (string.IsNullOrWhiteSpace(axisId)) throw new ArgumentException("axisId is required", nameof(axisId));
            AxisId = axisId;
        }

        public string AxisId { get; private set; }

        public int SeroOn()
        {
            lock (gate) servoOn = true;
            return 0;
        }

        public int SeroOff()
        {
            lock (gate) servoOn = false;
            return 0;
        }

        public bool isServoOn()
        {
            lock (gate) return servoOn;
        }

        public bool isServoOff()
        {
            lock (gate) return !servoOn;
        }

        public int Stop()
        {
            lock (gate) movingHome = false;
            return 0;
        }

        public int MoveHome()
        {
            lock (gate)
            {
                movingHome = true;
                actualPosition = 0;
                movingHome = false;
            }
            return 0;
        }

        public bool IsMoveHomeDone()
        {
            lock (gate) return !movingHome;
        }

        public int MoveAbsolute(double position)
        {
            lock (gate) actualPosition = position;
            return 0;
        }

        public int MoveRelative(double position)
        {
            lock (gate) actualPosition += position;
            return 0;
        }

        public double GetActualPosition()
        {
            lock (gate) return actualPosition;
        }
    }
}
