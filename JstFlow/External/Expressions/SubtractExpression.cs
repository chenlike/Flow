using JstFlow.Attributes;
using JstFlow.Internal.Metas;
using System;

namespace JstFlow.External.Expressions
{
    [FlowExpr("减法")]
    public class SubtractExpression<T> : FlowExpression<T> where T : struct
    {
        [Input("左操作数")]
        public T Left { get; set; }

        [Input("右操作数")]
        public T Right { get; set; }

        public override T Evaluate()
        {
            return (T)((dynamic)Left - (dynamic)Right);
        }
    }
} 