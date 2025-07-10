using JstFlow.Attributes;
using JstFlow.Internal.Metas;
using System;

namespace JstFlow.External.Expressions
{
    [FlowExpr("文本拼接")]
    public class ConcatExpression : FlowExpression<string>
    {
        [Input("文本1")]
        public string String1 { get; set; }

        [Input("文本2")]
        public string String2 { get; set; }

        [Input("文本3")]
        public string String3 { get; set; }

        [Input("文本4")]
        public string String4 { get; set; }

        [Input("文本5")]
        public string String5 { get; set; }

        public override string Evaluate()
        {
            return (String1 ?? "") + (String2 ?? "") + (String3 ?? "") + (String4 ?? "") + (String5 ?? "");
        }
    }
} 