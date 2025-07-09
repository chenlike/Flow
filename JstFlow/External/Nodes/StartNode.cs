using JstFlow.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.External
{
    /// <summary>
    /// 开始节点 每个图都必须要有
    /// </summary>
    [FlowNode("开始")]
    public class StartNode
    {
        [Emit("开始执行")]
        public event Action Start;
    }
}
