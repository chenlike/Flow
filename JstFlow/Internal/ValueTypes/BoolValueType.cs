using JstFlow.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.Internal.ValueTypes
{
    /// <summary>
    /// 布尔值类型定义 - 单例模式
    /// </summary>
    public class BoolType : IValueType
    {
        private static readonly BoolType _instance = new BoolType();
        
        // 私有构造函数，防止外部实例化
        private BoolType() { }
        
        // 单例实例
        public static BoolType Instance => _instance;

        public string ValueName => "布尔值";
        public string ValueCode => "bool";

        public bool IsValid<T>(T value)
        {
            if (value is bool)
                return true;

            try
            {
                Convert.ToBoolean(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IValue CreateValue<T>(T value)
        {
            return new BoolValue(value);
        }

        public bool Equals(IValueType other)
        {
            return other is BoolType && other.ValueCode == ValueCode;
        }
    }

    /// <summary>
    /// 布尔值实例
    /// </summary>
    public class BoolValue : IValue
    {
        private bool _value;

        public IValueType ValueType => BoolType.Instance;

        // 强类型构造函数
        public BoolValue(bool value)
        {
            _value = value;
        }

        // 从其他类型转换的构造函数
        public BoolValue(object value)
        {
            if (value is bool b)
            {
                _value = b;
            }
            else
            {
                try
                {
                    _value = Convert.ToBoolean(value);
                }
                catch
                {
                    _value = false;
                }
            }
        }

        public override string ToString()
        {
            return _value.ToString().ToLower();
        }

        public T GetValue<T>()
        {
            if (typeof(T) == typeof(bool))
                return (T)(object)_value;

            try
            {
                return (T)Convert.ChangeType(_value, typeof(T));
            }
            catch
            {
                return default;
            }
        }

        public bool SetValue<T>(T value)
        {
            if (value is bool b)
            {
                _value = b;
                return true;
            }

            try
            {
                _value = Convert.ToBoolean(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IsValid<T>(T value)
        {
            return ValueType.IsValid(value);
        }

        public bool Equals(IValue other)
        {
            if (other is BoolValue boolValue)
            {
                return boolValue.ValueType.Equals(ValueType) && 
                       boolValue.GetValue<bool>() == _value;
            }
            return false;
        }
    }
} 