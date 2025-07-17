using JstFlow.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.External.Nodes
{
    [FlowNode("DEBUG打印日志")]
    public class DebugLogNode:FlowBaseNode
    {
        [FlowInput("内容")]
        public string Content { get; set; }


        [FlowEvent("下一步")]
        public FlowEndpoint Next { get; set; }

        [FlowSignal("打印")]
        public FlowOutEvent Print()
        {
            Console.WriteLine($"[DEBUG] {Content}");
            return Emit(() => Next);
        }

    }
}
