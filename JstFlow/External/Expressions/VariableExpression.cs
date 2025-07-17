using JstFlow.Attributes;
using JstFlow.Core.Metas;
using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.External.Expressions
{
    [FlowExpr("变量")]
    public class VariableExpression<T> : FlowExpression<T>
    {
        public VariableExpression(T value = default(T))
        {
            Value = value;
        }

        [FlowInput("值")]
        public T Value { get; set; }

        public override T Evaluate()
        {
            return Value;
        }
    }
}
