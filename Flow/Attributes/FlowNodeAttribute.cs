using System;
using System.Collections.Generic;
using System.Text;

namespace Flow.Attributes
{
    /// <summary>
    /// 节点
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class FlowNodeAttribute : Attribute
    {
        public string Label { get; }

        public FlowNodeAttribute(string label = "")
        {
            Label = label;
        }


    }
}
