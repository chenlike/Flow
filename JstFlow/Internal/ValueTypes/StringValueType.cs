using JstFlow.Interface;
using System;

namespace JstFlow.Internal.ValueTypes
{
    /// <summary>
    /// 字符串值类型定义 - 单例模式
    /// </summary>
    public class StringType : IValueType
    {
        private static readonly StringType _instance = new StringType();
        
        // 私有构造函数，防止外部实例化
        private StringType() { }
        
        // 单例实例
        public static StringType Instance => _instance;

        public string ValueName => "字符串";
        public string ValueCode => "string";

        public bool IsValid<T>(T value)
        {
            return value is string str && !string.IsNullOrWhiteSpace(str);
        }

        public IValue CreateValue<T>(T value)
        {
            return new StringValue(value);
        }

        public bool Equals(IValueType other)
        {
            return other is StringType && other.ValueCode == ValueCode;
        }
    }

    /// <summary>
    /// 字符串值实例
    /// </summary>
    public class StringValue : IValue
    {
        private string _value;

        public IValueType ValueType => StringType.Instance;

        // 强类型构造函数
        public StringValue(string value)
        {
            _value = value ?? string.Empty;
        }

        // 从其他类型转换的构造函数
        public StringValue(object value)
        {
            if (value is string str)
            {
                _value = str;
            }
            else
            {
                try
                {
                    _value = Convert.ToString(value) ?? string.Empty;
                }
                catch
                {
                    _value = string.Empty;
                }
            }
        }

        public override string ToString()
        {
            return _value;
        }

        public T GetValue<T>()
        {
            if (typeof(T) == typeof(string))
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
            if (value is string str)
            {
                _value = str;
                return true;
            }

            try
            {
                _value = Convert.ToString(value);
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
            if (other is StringValue stringValue)
            {
                return stringValue.ValueType.Equals(ValueType) && 
                       stringValue.GetValue<string>() == _value;
            }
            return false;
        }
    }
}
