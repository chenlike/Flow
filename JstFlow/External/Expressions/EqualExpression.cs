using JstFlow.Attributes;
using JstFlow.Internal.Metas;
using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.External.Expressions
{
    [FlowExpr("相等")]
    public class EqualExpression<T> : FlowExpression<bool> where T:IEquatable<T>
    {
        [Input("左操作数")]
        public T Left { get; set; }

        [Input("右操作数")]
        public T Right { get; set; }

        public override bool Evaluate()
        {
            return Left.Equals(Right); 
        }
    }
}
