using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.Attributes
{
    /// <summary>
    /// 输入字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class InputAttribute : Attribute
    {
        public string Label { get; set; }


        public bool Required { get; set; }

        public InputAttribute(string label = "")
        {
            Label = label;
        }
    }
}
