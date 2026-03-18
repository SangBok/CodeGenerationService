using SampleEquipment.Control;
using SampleEquipment.Hardware.Abstractions;
using SampleEquipment.System;

namespace SampleEquipment.Process;

/// <summary>
/// 예시설비 전체 시퀀스와 상태머신을 표현하는 간단한 예시 클래스.
/// </summary>
public sealed class PickPlaceProcess
{
    private readonly PickPlaceCycle _cycle;
    private readonly IIoController _io;

    public ProcessState State { get; private set; } = ProcessState.Idle;

    public PickPlaceProcess(PickPlaceCycle cycle, IIoController io)
    {
        _cycle = cycle;
        _io = io;
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            switch (State)
            {
                case ProcessState.Idle:
                    if (IsStartRequested())
                    {
                        State = ProcessState.Ready;
                    }
                    break;

                case ProcessState.Ready:
                    State = ProcessState.Running;
                    break;

                case ProcessState.Running:
                    try
                    {
                        await _cycle.ExecuteCycleAsync(IsPauseRequested, IsStartRequested, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch
                    {
                        State = ProcessState.Alarm;
                    }

                    break;

                case ProcessState.Alarm:
                    if (IsResetRequested())
                    {
                        State = ProcessState.Idle;
                    }
                    break;
            }

            await Task.Delay(10, cancellationToken);
        }
    }

    private bool IsStartRequested() => _io.ReadInput(HardwareIds.IoInputs.StartButton);
    private bool IsPauseRequested() => _io.ReadInput(HardwareIds.IoInputs.StopButton);
    private bool IsResetRequested() => _io.ReadInput(HardwareIds.IoInputs.ResetButton);
}

