using System;
using System.Collections.Generic;
using System.Text;
using JstFlow.Attributes;
using JstFlow.Core.Interfaces;
using JstFlow.Core.Metas;
using JstFlow.External.Nodes;

namespace JstFlow.External
{
    [FlowNode("For循环节点")]
    public class ForNode : FlowBaseNode
    {
        [FlowInput("起始值", Required = true)]
        public int Start { get; set; }

        [FlowInput("结束值", Required = true)]
        public int End { get; set; }

        [FlowInput("步长")]
        public int Step { get; set; } = 1;

        [FlowInput("中断条件")]
        public bool BreakCondition { get; set; }

        [FlowOutput("当前索引")]
        public int CurrentIndex { get; set; }

        [FlowOutput("是否完成")]
        public bool IsCompleted { get; set; }

        [FlowOutput("是否被中断")]
        public bool IsBreak { get; set; }

        [FlowEvent("循环体")]
        public FlowEndpoint LoopBody { get; set; }

        [FlowEvent("循环完成")]
        public FlowEndpoint LoopCompleted { get; set; }


        [FlowSignal("开始循环")]
        public FlowOutEvent StartLoop()
        {
            // 重置状态
            Reset();

            for (CurrentIndex = Start; CurrentIndex <= End; CurrentIndex += Step)
            {
                Execute(()=>LoopBody);
                if(BreakCondition)
                {
                    break;
                }
            }

            return MoveNext(()=>LoopCompleted);
        }

        [FlowSignal("重置")]
        public void Reset()
        {
            CurrentIndex = Start;
            IsCompleted = false;
            IsBreak = false;
            BreakCondition = false;
        }

        [FlowSignal("中断循环")]
        public void Break()
        {
            BreakCondition = true;
            IsBreak = true;
        }

    }
}
