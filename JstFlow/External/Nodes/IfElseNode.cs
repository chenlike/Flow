using System;
using System.Collections.Generic;
using System.Text;
using JstFlow.Attributes;
using JstFlow.Core.Metas;
using JstFlow.External.Nodes;

namespace JstFlow.External
{
    [FlowNode("If-Else条件节点")]
    public class IfElseNode:FlowBaseNode
    {
        [FlowInput("条件", Required = true)]
        public bool Condition { get; set; }


        [FlowEvent("真")]
        public FlowEndpoint TrueBranch { get; set; }

        [FlowEvent("假")]
        public FlowEndpoint FalseBranch { get; set; }


        [FlowSignal("执行")]
        public FlowOutEvent Execute()
        {
            if (Condition)
            {
                return MoveNext(() => TrueBranch);
            }
            else
            {
                return MoveNext(() => FalseBranch);
            }
        }
    }
}
