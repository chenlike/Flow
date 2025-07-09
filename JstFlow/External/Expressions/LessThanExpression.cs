using JstFlow.Attributes;
using JstFlow.Internal.Metas;
using System;

namespace JstFlow.External.Expressions
{
    [FlowExpr("小于")]
    public class LessThanExpression<T> : FlowExpression<bool> where T : IComparable<T>
    {
        [Input("左操作数")]
        public T Left { get; set; }

        [Input("右操作数")]
        public T Right { get; set; }

        public override bool Evaluate()
        {
            return Left.CompareTo(Right) < 0;
        }
    }
} 