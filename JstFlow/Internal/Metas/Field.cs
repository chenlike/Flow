using JstFlow.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.Internal.NodeMeta
{
    public class Field
    {
        /// <summary>
        /// 名称
        /// </summary>
        public Label Label { get; set; }

        /// <summary>
        /// 类型 
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 是否为泛型参数
        /// </summary>
        public bool IsGenericParameter { get; set; }

        /// <summary>
        /// 泛型参数名称（如果IsGenericParameter为true）
        /// </summary>
        public string GenericParameterName { get; set; }

        /// <summary>
        /// 泛型约束类型列表
        /// </summary>
        public List<string> GenericConstraints { get; set; } = new List<string>();
    }
}
