namespace SampleEquipment.System;

public sealed class IoChannelConfig
{
    public required string Name { get; init; }
    public required int Index { get; init; }
    public required string Direction { get; init; } // "input" or "output"
}

public static class IoConfigurations
{
    public const string IoBoardId = "MainIo";

    public static readonly IoChannelConfig StartButton = new()
    {
        Name = HardwareIds.IoInputs.StartButton,
        Index = 0,
        Direction = "input"
    };

    public static readonly IoChannelConfig StopButton = new()
    {
        Name = HardwareIds.IoInputs.StopButton,
        Index = 1,
        Direction = "input"
    };

    public static readonly IoChannelConfig ResetButton = new()
    {
        Name = HardwareIds.IoInputs.ResetButton,
        Index = 2,
        Direction = "input"
    };

    public static readonly IoChannelConfig PartPresentSensor = new()
    {
        Name = HardwareIds.IoInputs.PartPresentSensor,
        Index = 3,
        Direction = "input"
    };

    public static readonly IoChannelConfig PickPositionReached = new()
    {
        Name = HardwareIds.IoInputs.PickPositionReached,
        Index = 4,
        Direction = "input"
    };

    public static readonly IoChannelConfig PlacePositionReached = new()
    {
        Name = HardwareIds.IoInputs.PlacePositionReached,
        Index = 5,
        Direction = "input"
    };

    public static readonly IoChannelConfig StartLamp = new()
    {
        Name = HardwareIds.IoOutputs.StartLamp,
        Index = 8,
        Direction = "output"
    };

    public static readonly IoChannelConfig AlarmLamp = new()
    {
        Name = HardwareIds.IoOutputs.AlarmLamp,
        Index = 9,
        Direction = "output"
    };

    public static readonly IoChannelConfig Buzzer = new()
    {
        Name = HardwareIds.IoOutputs.Buzzer,
        Index = 10,
        Direction = "output"
    };

    public static readonly IoChannelConfig GripperOpenSol = new()
    {
        Name = HardwareIds.IoOutputs.GripperOpenSol,
        Index = 11,
        Direction = "output"
    };

    public static readonly IoChannelConfig GripperCloseSol = new()
    {
        Name = HardwareIds.IoOutputs.GripperCloseSol,
        Index = 12,
        Direction = "output"
    };
}

