using System.Threading.Tasks;

namespace AutomationTemplate._5_Utility.BtRuntime.Nodes
{
    internal interface INode
    {
        string Id { get; }
        string Type { get; }
        Task<BtNodeStatus> TickAsync(BtContext context);
    }
}

