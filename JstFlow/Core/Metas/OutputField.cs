using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace JstFlow.Core.NodeMeta
{
    public class OutputField : Field
    {
        public PropertyInfo PropertyInfo { get; set; }
    }
}
