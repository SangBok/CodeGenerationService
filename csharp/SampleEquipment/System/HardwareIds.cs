namespace SampleEquipment.System;

/// <summary>
/// 하드웨어 명세의 ID/이름을 .NET 코드에서 사용하는 상수/열거형으로 정리한 클래스.
/// </summary>
public static class HardwareIds
{
    public const string EquipmentId = "PickPlaceDemo01";

    public static class Axes
    {
        public const string AxisX = "AxisX";
        public const string AxisY = "AxisY";
        public const string AxisZ = "AxisZ";
    }

    public static class IoInputs
    {
        public const string StartButton = "StartButton";
        public const string StopButton = "StopButton";
        public const string ResetButton = "ResetButton";

        public const string PartPresentSensor = "PartPresentSensor";
        public const string PickPositionReached = "PickPositionReached";
        public const string PlacePositionReached = "PlacePositionReached";
    }

    public static class IoOutputs
    {
        public const string StartLamp = "StartLamp";
        public const string AlarmLamp = "AlarmLamp";
        public const string Buzzer = "Buzzer";

        public const string GripperOpenSol = "GripperOpenSol";
        public const string GripperCloseSol = "GripperCloseSol";
    }

    public static class Units
    {
        public const string PickPlaceUnit = "PickPlaceUnit";
    }
}

