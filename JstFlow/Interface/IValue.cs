using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.Interface
{
    /// <summary>
    /// 表示具体值的接口
    /// </summary>
    public interface IValue : IEquatable<IValue>
    {
        /// <summary>
        /// 获取此值所属的类型
        /// </summary>
        IValueType ValueType { get; }

        /// <summary>
        /// 获取指定类型的值
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>转换后的值</returns>
        T GetValue<T>();

        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="value">要设置的值</param>
        /// <returns>如果设置成功则返回true</returns>
        bool SetValue<T>(T value);

        /// <summary>
        /// 检查指定值是否可以设置到此值中
        /// </summary>
        /// <typeparam name="T">要检查的值类型</typeparam>
        /// <param name="value">要检查的值</param>
        /// <returns>如果值可以设置则返回true</returns>
        bool IsValid<T>(T value);

        /// <summary>
        /// 获取值的字符串表示
        /// </summary>
        /// <returns>值的字符串表示</returns>
        string ToString();
    }
}
