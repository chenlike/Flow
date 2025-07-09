using System;
using System.Collections.Generic;
using System.Text;
using JstFlow.Attributes;
using JstFlow.Internal.Metas;

namespace JstFlow.External
{
    [FlowNode("While循环节点")]
    public class WhileNode
    {
        [Input("循环条件", Required = true)]
        public bool Condition { get; set; }

        [Input("最大循环次数")]
        public int MaxIterations { get; set; } = 1000;

        [Output("当前迭代次数")]
        public int CurrentIteration { get; set; }

        [Output("是否完成")]
        public bool IsCompleted { get; set; }

        [Emit("循环体")]
        public event Action LoopBody;

        [Emit("循环完成")]
        public event Action LoopCompleted;

        [Signal("开始循环")]
        public void StartLoop()
        {
            IsCompleted = false;
            CurrentIteration = 0;

            while (Condition && CurrentIteration < MaxIterations)
            {
                CurrentIteration++;
                LoopBody?.Invoke();
            }

            IsCompleted = true;
            LoopCompleted?.Invoke();
        }

        [Signal("重置")]
        public void Reset()
        {
            CurrentIteration = 0;
            IsCompleted = false;
        }

        [Signal("停止循环")]
        public void StopLoop()
        {
            Condition = false;
        }
    }
}
