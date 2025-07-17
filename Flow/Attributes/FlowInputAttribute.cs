using System;
using System.Collections.Generic;
using System.Text;

namespace Flow.Attributes
{
    /// <summary>
    /// 输入字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FlowInputAttribute : Attribute
    {
        public string Label { get; set; }


        public bool Required { get; set; }

        public FlowInputAttribute(string label = "")
        {
            Label = label;
        }
    }
}
