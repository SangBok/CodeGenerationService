namespace SampleEquipment.Hardware.Abstractions;

public interface IIoController
{
    bool ReadInput(string name);
    void WriteOutput(string name, bool value);
}

