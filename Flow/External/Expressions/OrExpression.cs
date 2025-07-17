using Flow.Attributes;
using Flow.Core.Metas;
using System;

namespace Flow.External.Expressions
{
    [FlowExpr("或")]
    public class OrExpression : FlowExpression<bool>
    {
        [FlowInput("左操作数")]
        public bool Left { get; set; }

        [FlowInput("右操作数")]
        public bool Right { get; set; }

        public override bool Evaluate()
        {
            return Left || Right;
        }
    }
} 