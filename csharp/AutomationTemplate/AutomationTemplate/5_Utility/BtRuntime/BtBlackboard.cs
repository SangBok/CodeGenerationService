using System;
using System.Collections.Concurrent;
using Newtonsoft.Json.Linq;

namespace AutomationTemplate._5_Utility.BtRuntime
{
    public sealed class BtBlackboard
    {
        private readonly ConcurrentDictionary<string, object> values = new ConcurrentDictionary<string, object>(StringComparer.Ordinal);

        public void Set(string key, object value)
        {
            values[key] = value;
        }

        public bool TryGet(string key, out object value)
        {
            return values.TryGetValue(key, out value);
        }

        public object GetRequired(string key)
        {
            if (!values.TryGetValue(key, out var value))
                throw new InvalidOperationException("Blackboard key not found: " + key);
            return value;
        }

        public void SeedDefaultsFromSchema(BtDefinition definition)
        {
            if (definition == null || definition.BlackboardSchema == null) return;

            foreach (var key in definition.BlackboardSchema)
            {
                if (key == null) continue;
                if (string.IsNullOrWhiteSpace(key.Key)) continue;
                if (values.ContainsKey(key.Key)) continue;
                if (key.DefaultValue == null) continue;

                values[key.Key] = JTokenToDotNet(key.DefaultValue);
            }
        }

        public static object JTokenToDotNet(JToken token)
        {
            if (token == null) return null;

            if (token.Type == JTokenType.Object && token["$ref"] != null && token["$ref"].Type == JTokenType.String)
            {
                // caller must resolve refs using the blackboard
                return token;
            }

            switch (token.Type)
            {
                case JTokenType.Integer:
                    return token.Value<long>();
                case JTokenType.Float:
                    return token.Value<double>();
                case JTokenType.Boolean:
                    return token.Value<bool>();
                case JTokenType.String:
                    return token.Value<string>();
                case JTokenType.Null:
                    return null;
                default:
                    return token;
            }
        }
    }
}

