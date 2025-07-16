using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.Attributes
{
    /// <summary>
    /// 输出事件
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class FlowEventAttribute : Attribute
    {
        public string Label { get; }

        public FlowEventAttribute(string label = "")
        {
            Label = label;
        }
    }
}
