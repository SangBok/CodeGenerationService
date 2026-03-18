using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace AutomationTemplate._0_System.Config
{
    public sealed class HardwareConfig
    {
        public HardwareConfig(
            IoMapConfig ioMap,
            AxisGroupsConfig axisGroups,
            CylindersConfig cylinders,
            Dictionary<string, int> inputPortsByName,
            Dictionary<string, int> outputPortsByName,
            List<HardwareDeviceConfig> devices,
            Dictionary<string, HardwareDeviceConfig> deviceByKey)
        {
            IoMap = ioMap ?? throw new ArgumentNullException(nameof(ioMap));
            AxisGroups = axisGroups ?? throw new ArgumentNullException(nameof(axisGroups));
            Cylinders = cylinders ?? throw new ArgumentNullException(nameof(cylinders));
            InputPortsByName = inputPortsByName ?? throw new ArgumentNullException(nameof(inputPortsByName));
            OutputPortsByName = outputPortsByName ?? throw new ArgumentNullException(nameof(outputPortsByName));
            Devices = devices ?? throw new ArgumentNullException(nameof(devices));
            DeviceByKey = deviceByKey ?? throw new ArgumentNullException(nameof(deviceByKey));
        }

        public IoMapConfig IoMap { get; }
        public AxisGroupsConfig AxisGroups { get; }
        public CylindersConfig Cylinders { get; }

        public IReadOnlyDictionary<string, int> InputPortsByName { get; }
        public IReadOnlyDictionary<string, int> OutputPortsByName { get; }

        public IReadOnlyList<HardwareDeviceConfig> Devices { get; }
        public IReadOnlyDictionary<string, HardwareDeviceConfig> DeviceByKey { get; }

        public int ResolveInputPort(string name) => ResolvePort(InputPortsByName, name, "input");
        public int ResolveOutputPort(string name) => ResolvePort(OutputPortsByName, name, "output");

        /// <summary>
        /// Key format: "{type}:{id}" (ordinal, case-sensitive).
        /// </summary>
        public HardwareDeviceConfig GetDevice(string type, string id)
        {
            if (string.IsNullOrWhiteSpace(type)) throw new ArgumentException("type is required", nameof(type));
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("id is required", nameof(id));

            var key = type + ":" + id;
            if (!DeviceByKey.TryGetValue(key, out var device))
                throw new KeyNotFoundException($"Unknown device '{key}'. Check hardware.json devices.");
            return device;
        }

        private static int ResolvePort(IReadOnlyDictionary<string, int> map, string name, string kind)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException($"Invalid {kind} name (empty).");

            if (!map.TryGetValue(name, out var port))
                throw new KeyNotFoundException($"Unknown {kind} name '{name}'. Check io-map.json.");

            return port;
        }
    }

    public static class HardwareConfigLoader
    {
        public static HardwareConfig LoadFromConfigDirectory(string baseDirectory)
        {
            if (string.IsNullOrWhiteSpace(baseDirectory))
                throw new ArgumentException("baseDirectory is required", nameof(baseDirectory));

            var configDir = Path.Combine(baseDirectory, "config");
            var hardwarePath = Path.Combine(configDir, "hardware.json");

            IoMapConfig ioMap;
            AxisGroupsConfig axisGroups;
            CylindersConfig cylinders;
            List<HardwareDeviceConfig> devices;
            string sourceName;

            if (File.Exists(hardwarePath))
            {
                sourceName = "hardware.json";
                var all = ReadJson<HardwareConfigFile>(hardwarePath);
                ioMap = all.IoMap ?? throw new InvalidOperationException("hardware.json 'ioMap' is missing.");
                axisGroups = all.AxisGroups ?? throw new InvalidOperationException("hardware.json 'axisGroups' is missing.");
                cylinders = all.Cylinders ?? throw new InvalidOperationException("hardware.json 'cylinders' is missing.");
                devices = all.Devices ?? new List<HardwareDeviceConfig>();
            }
            else
            {
                sourceName = "split-config";
                var ioMapPath = Path.Combine(configDir, "io-map.json");
                var axisGroupsPath = Path.Combine(configDir, "axis-groups.json");
                var cylindersPath = Path.Combine(configDir, "cylinders.json");

                ioMap = ReadJson<IoMapConfig>(ioMapPath);
                axisGroups = ReadJson<AxisGroupsConfig>(axisGroupsPath);
                cylinders = ReadJson<CylindersConfig>(cylindersPath);
                devices = new List<HardwareDeviceConfig>();
            }

            ValidateIoMap(ioMap, sourceName);
            ValidateAxisGroups(axisGroups, sourceName);
            ValidateCylinders(cylinders, sourceName);
            ValidateDevices(devices, sourceName);

            var inputPorts = BuildNameToPortMap(ioMap.Inputs, "input", sourceName);
            var outputPorts = BuildNameToPortMap(ioMap.Outputs, "output", sourceName);

            ValidateCylinderIoReferences(cylinders, inputPorts, outputPorts);

            var deviceByKey = BuildDeviceIndex(devices, sourceName);

            return new HardwareConfig(ioMap, axisGroups, cylinders, inputPorts, outputPorts, devices, deviceByKey);
        }

        private static T ReadJson<T>(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Config file not found: {path}", path);

            var json = File.ReadAllText(path);
            try
            {
                var obj = JsonConvert.DeserializeObject<T>(json);
                if (obj == null)
                    throw new InvalidOperationException($"Config file '{path}' is empty or invalid JSON.");
                return obj;
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Invalid JSON in '{path}': {ex.Message}", ex);
            }
        }

        private static void ValidateIoMap(IoMapConfig ioMap, string sourceName)
        {
            if (ioMap.Inputs == null) throw new InvalidOperationException($"IO map 'inputs' is missing ({sourceName}).");
            if (ioMap.Outputs == null) throw new InvalidOperationException($"IO map 'outputs' is missing ({sourceName}).");

            ValidateIoPoints(ioMap.Inputs, "inputs", sourceName);
            ValidateIoPoints(ioMap.Outputs, "outputs", sourceName);
        }

        private static void ValidateIoPoints(List<IoPoint> points, string sectionName, string sourceName)
        {
            for (var i = 0; i < points.Count; i++)
            {
                var p = points[i];
                if (p == null) throw new InvalidOperationException($"IO map '{sectionName}[{i}]' is null ({sourceName}).");
                if (string.IsNullOrWhiteSpace(p.Name))
                    throw new InvalidOperationException($"IO map '{sectionName}[{i}].name' is required ({sourceName}).");
                if (p.Port < 0)
                    throw new InvalidOperationException($"IO map '{sectionName}[{i}].port' must be >= 0 ({sourceName}).");
            }
        }

        private static Dictionary<string, int> BuildNameToPortMap(List<IoPoint> points, string kind, string sourceName)
        {
            var map = new Dictionary<string, int>(StringComparer.Ordinal);
            var portSeen = new HashSet<int>();

            foreach (var p in points)
            {
                if (map.ContainsKey(p.Name))
                    throw new InvalidOperationException($"Duplicate {kind} name '{p.Name}' ({sourceName}).");
                if (!portSeen.Add(p.Port))
                    throw new InvalidOperationException($"Duplicate {kind} port '{p.Port}' ({sourceName}).");
                map[p.Name] = p.Port;
            }

            return map;
        }

        private static void ValidateAxisGroups(AxisGroupsConfig axisGroups, string sourceName)
        {
            if (axisGroups.AxisGroups == null)
                throw new InvalidOperationException($"Axis groups 'axisGroups' is missing ({sourceName}).");

            for (var i = 0; i < axisGroups.AxisGroups.Count; i++)
            {
                var g = axisGroups.AxisGroups[i];
                if (g == null) throw new InvalidOperationException($"Axis groups 'axisGroups[{i}]' is null ({sourceName}).");
                if (string.IsNullOrWhiteSpace(g.GroupName))
                    throw new InvalidOperationException($"Axis groups 'axisGroups[{i}].groupName' is required ({sourceName}).");
                if (g.Axes == null)
                    throw new InvalidOperationException($"Axis groups 'axisGroups[{i}].axes' is missing ({sourceName}).");
                for (var j = 0; j < g.Axes.Count; j++)
                {
                    if (string.IsNullOrWhiteSpace(g.Axes[j]))
                        throw new InvalidOperationException($"Axis groups 'axisGroups[{i}].axes[{j}]' is empty ({sourceName}).");
                }
            }
        }

        private static void ValidateCylinders(CylindersConfig cylinders, string sourceName)
        {
            if (cylinders.Cylinders == null)
                throw new InvalidOperationException($"Cylinders 'cylinders' is missing ({sourceName}).");

            var names = new HashSet<string>(StringComparer.Ordinal);
            for (var i = 0; i < cylinders.Cylinders.Count; i++)
            {
                var c = cylinders.Cylinders[i];
                if (c == null) throw new InvalidOperationException($"Cylinders 'cylinders[{i}]' is null ({sourceName}).");
                if (string.IsNullOrWhiteSpace(c.Name))
                    throw new InvalidOperationException($"Cylinders 'cylinders[{i}].name' is required ({sourceName}).");
                if (!names.Add(c.Name))
                    throw new InvalidOperationException($"Duplicate cylinder name '{c.Name}' ({sourceName}).");

                Require(c.OnSensor, $"Cylinders 'cylinders[{i}].onSensor' ({sourceName})");
                Require(c.OffSensor, $"Cylinders 'cylinders[{i}].offSensor' ({sourceName})");
                Require(c.OnSol, $"Cylinders 'cylinders[{i}].onSol' ({sourceName})");
                Require(c.OffSol, $"Cylinders 'cylinders[{i}].offSol' ({sourceName})");
            }
        }

        private static void Require(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException($"{fieldName} is required.");
        }

        private static void ValidateCylinderIoReferences(
            CylindersConfig cylinders,
            IReadOnlyDictionary<string, int> inputPorts,
            IReadOnlyDictionary<string, int> outputPorts)
        {
            foreach (var c in cylinders.Cylinders)
            {
                if (!inputPorts.ContainsKey(c.OnSensor))
                    throw new InvalidOperationException($"Cylinder '{c.Name}' references unknown input '{c.OnSensor}'.");
                if (!inputPorts.ContainsKey(c.OffSensor))
                    throw new InvalidOperationException($"Cylinder '{c.Name}' references unknown input '{c.OffSensor}'.");
                if (!outputPorts.ContainsKey(c.OnSol))
                    throw new InvalidOperationException($"Cylinder '{c.Name}' references unknown output '{c.OnSol}'.");
                if (!outputPorts.ContainsKey(c.OffSol))
                    throw new InvalidOperationException($"Cylinder '{c.Name}' references unknown output '{c.OffSol}'.");
            }
        }

        private static void ValidateDevices(List<HardwareDeviceConfig> devices, string sourceName)
        {
            if (devices == null) throw new InvalidOperationException($"Devices list is null ({sourceName}).");

            for (var i = 0; i < devices.Count; i++)
            {
                var d = devices[i];
                if (d == null) throw new InvalidOperationException($"Devices[{i}] is null ({sourceName}).");
                if (string.IsNullOrWhiteSpace(d.Type))
                    throw new InvalidOperationException($"Devices[{i}].type is required ({sourceName}).");
                if (string.IsNullOrWhiteSpace(d.Id))
                    throw new InvalidOperationException($"Devices[{i}].id is required ({sourceName}).");
                if (d.Settings == null)
                    throw new InvalidOperationException($"Devices[{i}].settings is null ({sourceName}).");
            }
        }

        private static Dictionary<string, HardwareDeviceConfig> BuildDeviceIndex(List<HardwareDeviceConfig> devices, string sourceName)
        {
            var map = new Dictionary<string, HardwareDeviceConfig>(StringComparer.Ordinal);
            foreach (var d in devices)
            {
                var key = d.Type + ":" + d.Id;
                if (map.ContainsKey(key))
                    throw new InvalidOperationException($"Duplicate device key '{key}' ({sourceName}).");
                map[key] = d;
            }
            return map;
        }
    }
}
