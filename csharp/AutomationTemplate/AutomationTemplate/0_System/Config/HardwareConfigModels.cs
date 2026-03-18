using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AutomationTemplate._0_System.Config
{
    public sealed class HardwareConfigFile
    {
        [JsonProperty("schemaVersion")]
        public int SchemaVersion { get; set; } = 1;

        [JsonProperty("ioMap")]
        public IoMapConfig IoMap { get; set; }

        [JsonProperty("axisGroups")]
        public AxisGroupsConfig AxisGroups { get; set; }

        [JsonProperty("cylinders")]
        public CylindersConfig Cylinders { get; set; }

        /// <summary>
        /// Extensible device list (barcode, distance sensor, conveyor, etc.).
        /// Each entry should have a 'type' discriminator and a unique 'id'.
        /// </summary>
        [JsonProperty("devices")]
        public List<HardwareDeviceConfig> Devices { get; set; } = new List<HardwareDeviceConfig>();
    }

    public sealed class HardwareDeviceConfig
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Type-specific settings bag. Keep raw JSON to avoid loader changes
        /// when new device types are added.
        /// </summary>
        [JsonProperty("settings")]
        public JObject Settings { get; set; } = new JObject();
    }

    public sealed class IoMapConfig
    {
        [JsonProperty("inputs")]
        public List<IoPoint> Inputs { get; set; } = new List<IoPoint>();

        [JsonProperty("outputs")]
        public List<IoPoint> Outputs { get; set; } = new List<IoPoint>();
    }

    public sealed class IoPoint
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("port")]
        public int Port { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
    }

    public sealed class AxisGroupsConfig
    {
        [JsonProperty("axisGroups")]
        public List<AxisGroupConfig> AxisGroups { get; set; } = new List<AxisGroupConfig>();
    }

    public sealed class AxisGroupConfig
    {
        [JsonProperty("groupName")]
        public string GroupName { get; set; }

        [JsonProperty("axes")]
        public List<string> Axes { get; set; } = new List<string>();
    }

    public sealed class CylindersConfig
    {
        [JsonProperty("cylinders")]
        public List<CylinderConfig> Cylinders { get; set; } = new List<CylinderConfig>();
    }

    public sealed class CylinderConfig
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("onSensor")]
        public string OnSensor { get; set; }

        [JsonProperty("offSensor")]
        public string OffSensor { get; set; }

        [JsonProperty("onSol")]
        public string OnSol { get; set; }

        [JsonProperty("offSol")]
        public string OffSol { get; set; }
    }
}
