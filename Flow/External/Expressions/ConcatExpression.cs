using Flow.Attributes;
using Flow.Core.Metas;
using System;
using System.Collections.Generic;

namespace Flow.External.Expressions
{
    [FlowExpr("文本拼接")]
    public class ConcatExpression : FlowExpression<string>
    {
        [FlowInput("文本列表")]
        public List<string> StringList { get; set; }

        public override string Evaluate()
        {
            if(StringList == null)
            {
                return "";
            }
            return string.Join("", StringList);
        }
    }
} 