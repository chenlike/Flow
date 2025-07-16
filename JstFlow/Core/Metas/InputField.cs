using JstFlow.Common;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace JstFlow.Core.NodeMeta
{
    public class InputField : Field
    {


        /// <summary>
        /// 是否必填
        /// </summary>
        public bool Required { get; set; }

        public PropertyInfo PropertyInfo { get; set; }


    }
}
