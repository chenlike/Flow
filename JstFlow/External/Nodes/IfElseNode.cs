using System;
using System.Collections.Generic;
using System.Text;
using JstFlow.Attributes;
using JstFlow.Internal.Metas;

namespace JstFlow.External
{
    [FlowNode("If-Else条件节点")]
    public class IfElseNode
    {
        [Input("条件", Required = true)]
        public bool Condition { get; set; }

        [Emit("那么")]
        public event Action TrueBranch;

        [Emit("否则")]
        public event Action FalseBranch;

        [Signal("执行")]
        public void ExecuteCondition()
        {
            if (Condition)
            {
                TrueBranch?.Invoke();
            }
            else
            {
                FalseBranch?.Invoke();
            }
        }

    }
}
