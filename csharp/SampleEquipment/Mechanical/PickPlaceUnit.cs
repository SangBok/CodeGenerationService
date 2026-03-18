using SampleEquipment.Hardware.Abstractions;
using SampleEquipment.System;

namespace SampleEquipment.Mechanical;

/// <summary>
/// Pick&Place 유닛의 기본 단위 동작을 제공하는 예시 클래스.
/// </summary>
public sealed class PickPlaceUnit
{
    private readonly IAxisController _axisX;
    private readonly IAxisController _axisY;
    private readonly IAxisController _axisZ;
    private readonly IGripperController _gripper;
    private readonly IIoController _io;

    public PickPlaceUnit(
        IAxisController axisX,
        IAxisController axisY,
        IAxisController axisZ,
        IGripperController gripper,
        IIoController io)
    {
        _axisX = axisX;
        _axisY = axisY;
        _axisZ = axisZ;
        _gripper = gripper;
        _io = io;
    }

    public async Task MoveToHomeAsync(CancellationToken cancellationToken = default)
    {
        await _axisZ.MoveToPositionAsync(0, 100, cancellationToken);
        await _axisY.MoveToPositionAsync(0, 100, cancellationToken);
        await _axisX.MoveToPositionAsync(0, 100, cancellationToken);
    }

    public async Task MoveToPickPositionAsync(CancellationToken cancellationToken = default)
    {
        await _axisX.MoveToPositionAsync(100, 100, cancellationToken);
        await _axisY.MoveToPositionAsync(50, 100, cancellationToken);
        await _axisZ.MoveToPositionAsync(-50, 100, cancellationToken);
    }

    public async Task MoveToPlacePositionAsync(CancellationToken cancellationToken = default)
    {
        await _axisX.MoveToPositionAsync(200, 100, cancellationToken);
        await _axisY.MoveToPositionAsync(50, 100, cancellationToken);
        await _axisZ.MoveToPositionAsync(-50, 100, cancellationToken);
    }

    public Task GripAsync(CancellationToken cancellationToken = default)
    {
        return _gripper.CloseAsync(cancellationToken);
    }

    public Task ReleaseAsync(CancellationToken cancellationToken = default)
    {
        return _gripper.OpenAsync(cancellationToken);
    }

    public Task WaitForPartPresentAsync(CancellationToken cancellationToken = default)
    {
        return WaitForInputAsync(HardwareIds.IoInputs.PartPresentSensor, cancellationToken);
    }

    private async Task WaitForInputAsync(string inputName, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (_io.ReadInput(inputName))
            {
                return;
            }

            await Task.Delay(10, cancellationToken);
        }

        cancellationToken.ThrowIfCancellationRequested();
    }
}

