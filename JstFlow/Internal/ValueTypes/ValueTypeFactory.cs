using JstFlow.Interface;
using System;
using System.Collections.Generic;

namespace JstFlow.Internal.ValueTypes
{
    /// <summary>
    /// 值类型工厂类，提供统一的值类型访问接口
    /// </summary>
    public static class ValueTypeFactory
    {
        private static readonly Dictionary<string, IValueType> _valueTypes = new Dictionary<string, IValueType>
        {
            { "bool", BoolType.Instance },
            { "string", StringType.Instance },
            { "number", NumberType.Instance },
            { "list", ListType.Instance }
        };

        /// <summary>
        /// 获取所有支持的值类型
        /// </summary>
        public static IEnumerable<IValueType> GetAllValueTypes()
        {
            return _valueTypes.Values;
        }

        /// <summary>
        /// 根据类型代码获取值类型
        /// </summary>
        /// <param name="typeCode">类型代码</param>
        /// <returns>对应的值类型，如果不存在则返回null</returns>
        public static IValueType GetValueType(string typeCode)
        {
            return _valueTypes.TryGetValue(typeCode, out var valueType) ? valueType : null;
        }

        /// <summary>
        /// 检查指定的类型代码是否支持
        /// </summary>
        /// <param name="typeCode">类型代码</param>
        /// <returns>如果支持则返回true</returns>
        public static bool IsSupportedType(string typeCode)
        {
            return _valueTypes.ContainsKey(typeCode);
        }

        /// <summary>
        /// 创建指定类型的值实例
        /// </summary>
        /// <param name="typeCode">类型代码</param>
        /// <param name="value">初始值</param>
        /// <returns>新创建的值实例，如果类型不支持则返回null</returns>
        public static IValue CreateValue(string typeCode, object value)
        {
            var valueType = GetValueType(typeCode);
            return valueType?.CreateValue(value);
        }

        /// <summary>
        /// 获取布尔值类型
        /// </summary>
        public static BoolType BoolType => BoolType.Instance;

        /// <summary>
        /// 获取字符串类型
        /// </summary>
        public static StringType StringType => StringType.Instance;

        /// <summary>
        /// 获取数字类型
        /// </summary>
        public static NumberType NumberType => NumberType.Instance;

        /// <summary>
        /// 获取列表类型
        /// </summary>
        public static ListType ListType => ListType.Instance;
    }
} 