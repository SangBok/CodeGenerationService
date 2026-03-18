using System.Collections.Concurrent;
using System.Text.Json;

namespace Bt.Runtime;

public sealed class BtBlackboard
{
    private readonly ConcurrentDictionary<string, object?> _values = new(StringComparer.Ordinal);

    public bool TryGet<T>(string key, out T value)
    {
        if (_values.TryGetValue(key, out var obj) && obj is T t)
        {
            value = t;
            return true;
        }

        value = default!;
        return false;
    }

    public T GetRequired<T>(string key)
    {
        if (!TryGet<T>(key, out var value))
            throw new InvalidOperationException($"Blackboard key '{key}' is missing or not of type {typeof(T).Name}.");

        return value;
    }

    public void Set<T>(string key, T value) => _values[key] = value;

    public void SeedFromBlackboardSchema(IEnumerable<BtDefinition.BlackboardKey>? schema)
    {
        if (schema is null) return;

        foreach (var key in schema)
        {
            if (!key.DefaultValue.HasValue) continue;
            if (_values.ContainsKey(key.Key)) continue;

            var dv = key.DefaultValue.Value;
            var seeded = BtValueResolver.ToDotNetObject(dv.ValueKind, dv);
            _values[key.Key] = seeded;
        }
    }
}

internal static class BtValueResolver
{
    public static object? ResolveValue(JsonElement element, BtBlackboard blackboard)
    {
        if (element.ValueKind == JsonValueKind.Object &&
            element.TryGetProperty("$ref", out var refProp) &&
            refProp.ValueKind == JsonValueKind.String)
        {
            var key = refProp.GetString()!;
            if (!blackboard.TryGet<object?>(key, out var resolved))
                throw new InvalidOperationException($"Blackboard ref '{key}' was not found.");

            return resolved;
        }

        return ToDotNetObject(element.ValueKind, element);
    }

    public static object? ToDotNetObject(JsonValueKind kind, JsonElement element)
    {
        return kind switch
        {
            JsonValueKind.Null => null,
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.TryGetInt32(out var i) ? i : element.GetDouble(),
            _ => element.GetRawText()
        };
    }
}

