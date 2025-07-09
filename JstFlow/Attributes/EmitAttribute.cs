using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.Attributes
{
    /// <summary>
    /// 输出事件
    /// </summary>
    [AttributeUsage(AttributeTargets.Event, Inherited = true, AllowMultiple = false)]
    public class EmitAttribute : Attribute
    {
        public string Label { get; }

        public EmitAttribute(string label = "")
        {
            Label = label;
        }
    }
}
