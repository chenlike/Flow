using JstFlow.Attributes;
using JstFlow.Core.Metas;
using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.External.Expressions
{
    [FlowExpr("相等")]
    public class EqualExpression<T> : FlowExpression<bool> where T:IEquatable<T>
    {
        [FlowInput("左操作数")]
        public T Left { get; set; }

        [FlowInput("右操作数")]
        public T Right { get; set; }

        public override bool Evaluate()
        {
            if (Left == null && Right == null)
                return true;
            if (Left == null || Right == null)
                return false;
            return Left.Equals(Right); 
        }
    }
}
