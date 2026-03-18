using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AutomationTemplate._5_Utility.BtRuntime
{
    public sealed class BtDefinition
    {
        [JsonProperty("schemaVersion")]
        public string SchemaVersion { get; set; }

        [JsonProperty("treeId")]
        public string TreeId { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("rootNodeId")]
        public string RootNodeId { get; set; }

        [JsonProperty("nodes")]
        public List<Node> Nodes { get; set; }

        [JsonProperty("blackboardSchema")]
        public List<BlackboardKey> BlackboardSchema { get; set; }

        [JsonProperty("bindings")]
        public JObject Bindings { get; set; }

        [JsonProperty("metadata")]
        public JObject Metadata { get; set; }

        public static BtDefinition LoadFromFile(string path)
        {
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<BtDefinition>(json);
        }

        public sealed class Node
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("children")]
            public List<string> Children { get; set; }

            [JsonProperty("parameters")]
            public JObject Parameters { get; set; }

            [JsonProperty("guard")]
            public string Guard { get; set; }
        }

        public sealed class BlackboardKey
        {
            [JsonProperty("key")]
            public string Key { get; set; }

            [JsonProperty("valueType")]
            public string ValueType { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("defaultValue")]
            public JToken DefaultValue { get; set; }
        }
    }
}

