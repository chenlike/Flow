using System;
using System.Collections.Generic;
using System.Text;

namespace Flow.Attributes
{
    /// <summary>
    /// 输出字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FlowOutputAttribute : Attribute
    {
        public string Label { get; set; }

        public FlowOutputAttribute(string label = "")
        {
            Label = label;
        }
    }
}
