using JstFlow.Attributes;
using JstFlow.External.Nodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.External
{
    /// <summary>
    /// 开始节点 每个图都必须要有
    /// </summary>
    [FlowNode("开始")]
    public class StartNode:FlowBaseNode
    {
        [FlowEvent("开始执行")]
        public FlowEndpoint Start { get; set; }

        [FlowSignal("开始循环")]
        public FlowOutEvent Execute()
        {
            return Emit(()=>Start);
        }
    }
}
