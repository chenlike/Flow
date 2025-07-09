using System;
using System.Collections.Generic;
using System.Text;
using JstFlow.Attributes;

namespace JstFlow.Internal.Metas
{
    public abstract class FlowExpression
    {
        [Signal("执行")]
        public abstract void Evaluate();
    }
}
