using JstFlow.Attributes;
using JstFlow.Core.Metas;
using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.External.Expressions
{
    public class VariableExpression<T> : FlowExpression<T>
    {
        [FlowInput("值")]
        public T Value { get; set; }

        public override T Evaluate()
        {
            return Value;
        }
    }
}
