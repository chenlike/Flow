using JstFlow.Attributes;
using JstFlow.Core.Metas;
using System;

namespace JstFlow.External.Expressions
{
    [FlowExpr("大于")]
    public class GreaterThanExpression<T> : FlowExpression<bool> where T : IComparable<T>
    {
        [FlowInput("左操作数")]
        public T Left { get; set; }

        [FlowInput("右操作数")]
        public T Right { get; set; }

        public override bool Evaluate()
        {
            return Left.CompareTo(Right) > 0;
        }
    }
} 