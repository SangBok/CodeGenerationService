using System;

namespace AutomationTemplate._5_Utility.BtRuntime
{
    public enum BtNodeStatus
    {
        Running = 0,
        Success = 1,
        Failure = 2
    }

    public sealed class BtTraceEventArgs : EventArgs
    {
        public BtTraceEventArgs(DateTime timestampUtc, string nodeId, string nodeName, string nodeType, string eventType, BtNodeStatus? status, string message)
        {
            TimestampUtc = timestampUtc;
            NodeId = nodeId;
            NodeName = nodeName;
            NodeType = nodeType;
            EventType = eventType;
            Status = status;
            Message = message;
        }

        public DateTime TimestampUtc { get; }
        public string NodeId { get; }
        public string NodeName { get; }
        public string NodeType { get; }
        public string EventType { get; }
        public BtNodeStatus? Status { get; }
        public string Message { get; }
    }
}

