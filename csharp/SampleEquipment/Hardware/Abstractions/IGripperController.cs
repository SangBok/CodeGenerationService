namespace SampleEquipment.Hardware.Abstractions;

public interface IGripperController
{
    Task OpenAsync(CancellationToken cancellationToken = default);
    Task CloseAsync(CancellationToken cancellationToken = default);
}

