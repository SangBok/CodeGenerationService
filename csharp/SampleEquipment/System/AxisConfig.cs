namespace SampleEquipment.System;

/// <summary>
/// 축 구성 정보를 보관하는 간단한 설정 클래스 예시.
/// 실제 코드 생성 시에는 하드웨어 명세(JSON 등)에서 로드될 수 있다.
/// </summary>
public sealed class AxisConfig
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Type { get; init; }
    public required string Vendor { get; init; }
    public required string Protocol { get; init; }
    public required int NodeOrAddress { get; init; }
}

public static class AxisConfigurations
{
    public static readonly AxisConfig AxisX = new()
    {
        Id = HardwareIds.Axes.AxisX,
        Name = "PickPlace_X",
        Type = "linear",
        Vendor = "DemoServo",
        Protocol = "EtherCAT",
        NodeOrAddress = 1
    };

    public static readonly AxisConfig AxisY = new()
    {
        Id = HardwareIds.Axes.AxisY,
        Name = "PickPlace_Y",
        Type = "linear",
        Vendor = "DemoServo",
        Protocol = "EtherCAT",
        NodeOrAddress = 2
    };

    public static readonly AxisConfig AxisZ = new()
    {
        Id = HardwareIds.Axes.AxisZ,
        Name = "PickPlace_Z",
        Type = "linear",
        Vendor = "DemoServo",
        Protocol = "EtherCAT",
        NodeOrAddress = 3
    };
}

