using JstFlow.Attributes;
using JstFlow.Internal.Metas;
using System;

namespace JstFlow.External.Expressions
{
    [FlowExpr("或")]
    public class OrExpression : FlowExpression<bool>
    {
        [Input("左操作数")]
        public bool Left { get; set; }

        [Input("右操作数")]
        public bool Right { get; set; }

        public override bool Evaluate()
        {
            return Left || Right;
        }
    }
} 