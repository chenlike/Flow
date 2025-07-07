using System;

namespace JstFlow.Interface
{
    /// <summary>
    /// 表示值类型的定义接口
    /// </summary>
    public interface IValueType : IEquatable<IValueType>
    {
        /// <summary>
        /// 获取类型名称
        /// </summary>
        string ValueName { get; }

        /// <summary>
        /// 获取类型代码
        /// </summary>
        string ValueCode { get; }

        /// <summary>
        /// 检查指定值是否属于此类型
        /// </summary>
        /// <typeparam name="T">要检查的值类型</typeparam>
        /// <param name="value">要检查的值</param>
        /// <returns>如果值属于此类型则返回true</returns>
        bool IsValid<T>(T value);

        /// <summary>
        /// 创建此类型的新实例
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="value">初始值</param>
        /// <returns>新创建的值实例</returns>
        IValue CreateValue<T>(T value);
    }
} 