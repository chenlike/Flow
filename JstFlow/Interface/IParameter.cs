using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.Interface
{
    public interface IParameter
    {

        /// <summary>
        /// 参数类型
        /// </summary>
        IValue ValueType { get; }

        /// <summary>
        /// 是否必填
        /// </summary>
        bool Required { get; }


    }
}
