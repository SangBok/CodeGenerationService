using System.Text.Json;

namespace Bt.Runtime;

public sealed class BtParam<T>
{
    public BtParam(string name, JsonElement element, Func<object?, T> convert)
    {
        Name = name;
        _element = element;
        _convert = convert;
    }

    public string Name { get; }
    private readonly JsonElement _element;
    private readonly Func<object?, T> _convert;

    public T Resolve(BtContext context)
    {
        var raw = BtValueResolver.ResolveValue(_element, context.Blackboard);
        return _convert(raw);
    }
}

