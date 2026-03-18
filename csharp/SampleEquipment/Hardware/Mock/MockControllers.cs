using SampleEquipment.Hardware.Abstractions;

namespace SampleEquipment.Hardware.Mock;

/// <summary>
/// 예시 설비용 Mock 축 제어기. 실제 하드웨어 대신 로그/딜레이만 수행한다.
/// </summary>
public sealed class MockAxisController : IAxisController
{
    public string AxisId { get; }

    public MockAxisController(string axisId)
    {
        AxisId = axisId;
    }

    public Task HomeAsync(CancellationToken cancellationToken = default)
    {
        return Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
    }

    public Task MoveToPositionAsync(double position, double speed, CancellationToken cancellationToken = default)
    {
        return Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
    }

    public Task StopAsync()
    {
        return Task.CompletedTask;
    }
}

public sealed class MockIoController : IIoController
{
    private readonly Dictionary<string, bool> _states = new();

    public bool ReadInput(string name)
    {
        return _states.TryGetValue(name, out var value) && value;
    }

    public void WriteOutput(string name, bool value)
    {
        _states[name] = value;
    }
}

public sealed class MockGripperController : IGripperController
{
    private readonly IIoController _io;

    public MockGripperController(IIoController io)
    {
        _io = io;
    }

    public Task OpenAsync(CancellationToken cancellationToken = default)
    {
        _io.WriteOutput(System.HardwareIds.IoOutputs.GripperOpenSol, true);
        _io.WriteOutput(System.HardwareIds.IoOutputs.GripperCloseSol, false);
        return Task.Delay(TimeSpan.FromMilliseconds(50), cancellationToken);
    }

    public Task CloseAsync(CancellationToken cancellationToken = default)
    {
        _io.WriteOutput(System.HardwareIds.IoOutputs.GripperOpenSol, false);
        _io.WriteOutput(System.HardwareIds.IoOutputs.GripperCloseSol, true);
        return Task.Delay(TimeSpan.FromMilliseconds(50), cancellationToken);
    }
}

