using Flow.Attributes;
using Flow.Core.Metas;
using System;

namespace Flow.External.Expressions
{
    [FlowExpr("减法")]
    public class SubtractExpression<T> : FlowExpression<T> where T : struct
    {
        [FlowInput("左操作数")]
        public T Left { get; set; }

        [FlowInput("右操作数")]
        public T Right { get; set; }

        public override T Evaluate()
        {
            return (T)((dynamic)Left - (dynamic)Right);
        }
    }
} 