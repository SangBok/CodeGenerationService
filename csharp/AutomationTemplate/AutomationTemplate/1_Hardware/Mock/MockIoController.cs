using System;

namespace AutomationTemplate._1_Hardware.Mock
{
    public sealed class MockIoController : HIIoController
    {
        private readonly bool[] inputs;
        private readonly bool[] outputs;
        private readonly object gate = new object();

        public MockIoController(int inputPortCount = 256, int outputPortCount = 256)
        {
            if (inputPortCount <= 0) throw new ArgumentOutOfRangeException(nameof(inputPortCount));
            if (outputPortCount <= 0) throw new ArgumentOutOfRangeException(nameof(outputPortCount));

            inputs = new bool[inputPortCount];
            outputs = new bool[outputPortCount];
        }

        public bool IsInputOn(int port)
        {
            lock (gate) return inputs[port];
        }

        public bool IsInputOff(int port)
        {
            lock (gate) return !inputs[port];
        }

        public int OutputOn(int port)
        {
            lock (gate) outputs[port] = true;
            return 0;
        }

        public int OutputOff(int port)
        {
            lock (gate) outputs[port] = false;
            return 0;
        }

        public bool IsOutputOn(int port)
        {
            lock (gate) return outputs[port];
        }

        public bool IsOutputOff(int port)
        {
            lock (gate) return !outputs[port];
        }

        public void SetInput(int port, bool value)
        {
            lock (gate) inputs[port] = value;
        }
    }
}
