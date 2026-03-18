namespace SampleEquipment.Hardware.Abstractions;

public interface IAxisController
{
    string AxisId { get; }

    Task HomeAsync(CancellationToken cancellationToken = default);
    Task MoveToPositionAsync(double position, double speed, CancellationToken cancellationToken = default);
    Task StopAsync();
}

