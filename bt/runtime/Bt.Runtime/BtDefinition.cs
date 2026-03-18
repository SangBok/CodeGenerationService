using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bt.Runtime;

public sealed class BtDefinition
{
    public string SchemaVersion { get; set; } = "1.0.0";
    public string TreeId { get; set; } = "";
    public int Version { get; set; } = 1;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string RootNodeId { get; set; } = "";
    public List<Node> Nodes { get; set; } = new();
    public List<BlackboardKey>? BlackboardSchema { get; set; }
    public JsonElement? Bindings { get; set; }
    public JsonElement? Metadata { get; set; }

    public static BtDefinition LoadFromFile(string path)
    {
        var json = File.ReadAllText(path);
        var def = JsonSerializer.Deserialize<BtDefinition>(json, JsonOptions)
                  ?? throw new InvalidOperationException("Failed to deserialize BT definition.");
        return def;
    }

    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public sealed class Node
    {
        public string Id { get; set; } = "";
        public string Type { get; set; } = "";
        public string? Name { get; set; }
        public List<string>? Children { get; set; }
        public Dictionary<string, JsonElement>? Parameters { get; set; }
        public string? Guard { get; set; }
    }

    public sealed class BlackboardKey
    {
        public string Key { get; set; } = "";
        public string ValueType { get; set; } = "";
        public string? Description { get; set; }
        public JsonElement? DefaultValue { get; set; }
    }
}

