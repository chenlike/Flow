using JstFlow.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.Internal.Base
{
    public class Parameter : IParameter
    {

        public Parameter(IValue valueType, bool required)
        {
            ValueType = valueType;
            Required = required;
        }

        public IValue ValueType { get; set; }

        public bool Required { get; set; }
    }
}
