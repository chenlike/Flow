using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.Attributes
{
    /// <summary>
    /// 信号
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class FlowSignalAttribute : Attribute
    {
        public string Label { get; }

        public FlowSignalAttribute(string label = "")
        {
            Label = label;
        }
    }
}
