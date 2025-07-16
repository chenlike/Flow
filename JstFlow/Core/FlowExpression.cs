using System;
using System.Collections.Generic;
using System.Text;
using JstFlow.Attributes;

namespace JstFlow.Core.Metas
{
    public abstract class FlowExpression<TResult>
    {
        public abstract TResult Evaluate();
    }
}
