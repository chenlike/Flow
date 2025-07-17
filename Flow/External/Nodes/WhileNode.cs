using System;
using System.Collections.Generic;
using System.Text;
using Flow.Attributes;
using Flow.Core.Metas;
using Flow.External.Nodes;

namespace Flow.External
{
    [FlowNode("While循环节点")]
    public class WhileNode : FlowBaseNode
    {
        [FlowInput("循环条件", Required = true)]
        public bool Condition { get; set; }

        [FlowInput("最大循环次数")]
        public int MaxIterations { get; set; } = 1000;

        [FlowOutput("当前迭代次数")]
        public int CurrentIteration { get; set; }

        [FlowOutput("是否完成")]
        public bool IsCompleted { get; set; }

        [FlowEvent("循环体")]
        public FlowEndpoint LoopBody { get; set; }

        [FlowEvent("循环完成")]
        public FlowEndpoint LoopCompleted { get; set; }


        private bool _isBreak;

        [FlowSignal("开始循环")]
        public IEnumerable<FlowOutEvent> StartLoop()
        {
            _isBreak = false;
            while (Condition && CurrentIteration < MaxIterations)
            {
                yield return Emit(()=>LoopBody);
                CurrentIteration++;
                if(_isBreak)
                {
                    break;
                }
            }
            yield return Emit(()=>LoopCompleted);
        }

        [FlowSignal("重置")]
        public void Reset()
        {
            CurrentIteration = 0;
            IsCompleted = false;
        }

        [FlowSignal("停止循环")]
        public void Break()
        {
            _isBreak = true;
        }

    }
}
