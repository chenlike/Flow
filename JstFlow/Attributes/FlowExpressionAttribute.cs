using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.Attributes
{
    /// <summary>
    /// 流表达式
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class FlowExpressionAttribute : Attribute
    {
        public string Label { get; }

        public FlowExpressionAttribute(string label = "")
        {
            Label = label;
        }
    }
} 