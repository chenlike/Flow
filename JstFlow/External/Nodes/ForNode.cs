using System;
using System.Collections.Generic;
using System.Text;
using JstFlow.Attributes;
using JstFlow.Internal.Metas;

namespace JstFlow.External
{
    [FlowNode("For循环节点")]
    public class ForNode
    {
        [Input("起始值", Required = true)]
        public int Start { get; set; }

        [Input("结束值", Required = true)]
        public int End { get; set; }

        [Input("步长")]
        public int Step { get; set; } = 1;

        [Input("中断条件")]
        public bool BreakCondition { get; set; }

        [Output("当前索引")]
        public int CurrentIndex { get; set; }

        [Output("是否完成")]
        public bool IsCompleted { get; set; }

        [Output("是否被中断")]
        public bool IsBreak { get; set; }

        [Emit("循环体")]
        public event Action LoopBody;

        [Signal("开始循环")]
        public void StartLoop()
        {
            IsCompleted = false;
            IsBreak = false;
            CurrentIndex = Start;
            
            while (CurrentIndex <= End)
            {
                // 检查中断条件
                if (BreakCondition)
                {
                    IsBreak = true;
                    break;
                }

                LoopBody?.Invoke();
                CurrentIndex += Step;
            }
            
            IsCompleted = true;
        }

        [Signal("重置")]
        public void Reset()
        {
            CurrentIndex = Start;
            IsCompleted = false;
            IsBreak = false;
        }

        [Signal("中断循环")]
        public void Break()
        {
            BreakCondition = true;
        }


    }
}
