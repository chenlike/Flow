using System;
using System.Collections.Generic;
using System.Text;
using Flow.Attributes;

namespace Flow.Core.Metas
{
    public abstract class FlowExpression<TResult>
    {
        public abstract TResult Evaluate();
    }
}
