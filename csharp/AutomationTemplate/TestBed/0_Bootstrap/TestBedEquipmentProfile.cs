using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace TestBed
{
    public sealed class TestBedEquipmentProfile
    {
        [JsonProperty("handlerAxes")]
        public HandlerAxesConfig HandlerAxes { get; set; } = new HandlerAxesConfig();

        [JsonProperty("stageAxes")]
        public StageAxesConfig StageAxes { get; set; } = new StageAxesConfig();

        [JsonProperty("cylinders")]
        public CylindersConfig Cylinders { get; set; } = new CylindersConfig();

        [JsonProperty("unitAliases")]
        public List<string> UnitAliases { get; set; } = new List<string>();

        [JsonProperty("ui")]
        public UiConfig Ui { get; set; } = new UiConfig();

        public static TestBedEquipmentProfile LoadFromConfigDirectory(string baseDirectory)
        {
            if (string.IsNullOrWhiteSpace(baseDirectory))
                throw new ArgumentException("baseDirectory is required", nameof(baseDirectory));

            var path = Path.Combine(baseDirectory, "config", "testbed-equipment.json");
            if (!File.Exists(path))
                throw new FileNotFoundException("TestBed equipment profile not found.", path);

            var json = File.ReadAllText(path);
            var profile = JsonConvert.DeserializeObject<TestBedEquipmentProfile>(json);
            if (profile == null)
                throw new InvalidOperationException("Invalid testbed-equipment.json.");

            profile.Validate();
            return profile;
        }

        private void Validate()
        {
            Require(HandlerAxes.X, "handlerAxes.x");
            Require(HandlerAxes.Y, "handlerAxes.y");
            Require(HandlerAxes.Z, "handlerAxes.z");

            Require(StageAxes.X, "stageAxes.x");
            Require(StageAxes.Y, "stageAxes.y");

            Require(Cylinders.Gripper, "cylinders.gripper");
            Require(Cylinders.SideAlign, "cylinders.sideAlign");
            Require(Cylinders.TopAlign, "cylinders.topAlign");
            Require(Cylinders.BottomAlign, "cylinders.bottomAlign");

            if (UnitAliases == null || UnitAliases.Count == 0)
                throw new InvalidOperationException("unitAliases must contain at least one alias.");
        }

        private static void Require(string value, string key)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException($"Missing required value: {key}");
        }

        public sealed class HandlerAxesConfig
        {
            [JsonProperty("x")]
            public string X { get; set; }

            [JsonProperty("y")]
            public string Y { get; set; }

            [JsonProperty("z")]
            public string Z { get; set; }
        }

        public sealed class StageAxesConfig
        {
            [JsonProperty("x")]
            public string X { get; set; }

            [JsonProperty("y")]
            public string Y { get; set; }
        }

        public sealed class CylindersConfig
        {
            [JsonProperty("gripper")]
            public string Gripper { get; set; }

            [JsonProperty("sideAlign")]
            public string SideAlign { get; set; }

            [JsonProperty("topAlign")]
            public string TopAlign { get; set; }

            [JsonProperty("bottomAlign")]
            public string BottomAlign { get; set; }
        }

        public sealed class UiConfig
        {
            [JsonProperty("safePositionIndex")]
            public int SafePositionIndex { get; set; } = 2;
        }
    }
}
