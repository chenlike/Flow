using JstFlow.Attributes;
using JstFlow.Core.Metas;
using System;

namespace JstFlow.External.Expressions
{
    [FlowExpr("并且")]
    public class AndExpression : FlowExpression<bool>
    {
        [FlowInput("左操作数")]
        public bool Left { get; set; }

        [FlowInput("右操作数")]
        public bool Right { get; set; }

        public override bool Evaluate()
        {
            return Left && Right;
        }
    }
} 