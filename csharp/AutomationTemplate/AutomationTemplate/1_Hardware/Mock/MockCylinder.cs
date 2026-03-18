using System;

namespace AutomationTemplate._1_Hardware.Mock
{
    public sealed class MockCylinder : HICylinder
    {
        private readonly HIIoController io;
        private readonly int onSensorPort;
        private readonly int offSensorPort;
        private readonly int onSolPort;
        private readonly int offSolPort;

        public MockCylinder(HIIoController io, int onSensorPort, int offSensorPort, int onSolPort, int offSolPort)
        {
            this.io = io ?? throw new ArgumentNullException(nameof(io));
            this.onSensorPort = onSensorPort;
            this.offSensorPort = offSensorPort;
            this.onSolPort = onSolPort;
            this.offSolPort = offSolPort;
        }

        public int Forward(bool isAsync = true)
        {
            io.OutputOn(onSolPort);
            io.OutputOff(offSolPort);
            return 0;
        }

        public int Backward(bool isAsync = true)
        {
            io.OutputOn(offSolPort);
            io.OutputOff(onSolPort);
            return 0;
        }

        public bool IsForward()
        {
            if (onSensorPort < 0) return io.IsOutputOn(onSolPort) && io.IsOutputOff(offSolPort);
            return io.IsInputOn(onSensorPort);
        }

        public bool IsBackward()
        {
            if (offSensorPort < 0) return io.IsOutputOn(offSolPort) && io.IsOutputOff(onSolPort);
            return io.IsInputOn(offSensorPort);
        }
    }
}
