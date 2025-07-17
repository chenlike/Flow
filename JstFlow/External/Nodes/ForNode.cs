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

        [FlowOutput("当前索引")]
        public int CurrentIndex { get; set; }

        [FlowOutput("是否完成")]
        public bool IsCompleted { get; set; }

        [FlowEvent("循环体")]
        public FlowEndpoint LoopBody { get; set; }

        [FlowEvent("循环完成")]
        public FlowEndpoint LoopCompleted { get; set; }



        private bool _isBreak { get; set; }


        [FlowSignal("开始循环")]
        public IEnumerable<FlowOutEvent> StartLoop()
        {
            // 重置状态
            Reset();
            

            for (CurrentIndex = Start; CurrentIndex < End; CurrentIndex += Step)
            {
                yield return Emit(()=>LoopBody);
                if(_isBreak)
                {
                    break;
                }
            }

            yield return Emit(()=>LoopCompleted);
            Console.WriteLine("循环完成");
        }

        [FlowSignal("重置")]
        public void Reset()
        {
            CurrentIndex = Start;
            IsCompleted = false;
            _isBreak = false;
        }

        [FlowSignal("中断循环")]
        public void Break()
        {
            _isBreak = true;
        }

    }
}
