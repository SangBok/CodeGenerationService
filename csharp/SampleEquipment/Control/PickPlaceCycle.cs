using SampleEquipment.Mechanical;

namespace SampleEquipment.Control;

/// <summary>
/// Pick&Place 1사이클(픽→플레이스)을 정의하는 예시 클래스.
/// Stop/Start 입력을 이용한 일시정지/재개를 지원하기 위해,
/// 각 단계 사이에서 pause/resume 조건을 검사한다.
/// </summary>
public sealed class PickPlaceCycle
{
    private readonly PickPlaceUnit _unit;

    public PickPlaceCycle(PickPlaceUnit unit)
    {
        _unit = unit;
    }

    public async Task ExecuteCycleAsync(
        Func<bool> isPauseRequested,
        Func<bool> isResumeRequested,
        CancellationToken cancellationToken = default)
    {
        await _unit.MoveToHomeAsync(cancellationToken);
        await WaitIfPausedAsync(isPauseRequested, isResumeRequested, cancellationToken);

        await _unit.WaitForPartPresentAsync(cancellationToken);
        await WaitIfPausedAsync(isPauseRequested, isResumeRequested, cancellationToken);

        await _unit.MoveToPickPositionAsync(cancellationToken);
        await WaitIfPausedAsync(isPauseRequested, isResumeRequested, cancellationToken);

        await _unit.GripAsync(cancellationToken);
        await WaitIfPausedAsync(isPauseRequested, isResumeRequested, cancellationToken);

        await _unit.MoveToPlacePositionAsync(cancellationToken);
        await WaitIfPausedAsync(isPauseRequested, isResumeRequested, cancellationToken);

        await _unit.ReleaseAsync(cancellationToken);
        await WaitIfPausedAsync(isPauseRequested, isResumeRequested, cancellationToken);

        await _unit.MoveToHomeAsync(cancellationToken);
        await WaitIfPausedAsync(isPauseRequested, isResumeRequested, cancellationToken);
    }

    private static async Task WaitIfPausedAsync(
        Func<bool> isPauseRequested,
        Func<bool> isResumeRequested,
        CancellationToken cancellationToken)
    {
        if (!isPauseRequested())
        {
            return;
        }

        // Stop 버튼이 눌린 상태에서는 Start 버튼이 다시 눌릴 때까지 대기한다.
        while (!cancellationToken.IsCancellationRequested)
        {
            if (isResumeRequested())
            {
                return;
            }

            await Task.Delay(10, cancellationToken);
        }

        cancellationToken.ThrowIfCancellationRequested();
    }
}

