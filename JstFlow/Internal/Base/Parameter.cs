using JstFlow.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.Internal.Base
{
    public class Parameter : IParameter
    {

        public Parameter(IValueType valueType, bool required)
        {
            ValueType = valueType;
            Required = required;
        }

        public IValueType ValueType { get; set; }

        public bool Required { get; set; }
    }
}
