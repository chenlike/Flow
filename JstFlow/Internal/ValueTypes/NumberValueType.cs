using JstFlow.Interface;
using System;

namespace JstFlow.Internal.ValueTypes
{
    /// <summary>
    /// 数字值类型定义 - 单例模式
    /// </summary>
    public class NumberType : IValueType
    {
        private static readonly NumberType _instance = new NumberType();
        
        // 私有构造函数，防止外部实例化
        private NumberType() { }
        
        // 单例实例
        public static NumberType Instance => _instance;

        public string ValueName => "数字";
        public string ValueCode => "number";

        public bool IsValid<T>(T value)
        {
            if (value is decimal)
                return true;

            try
            {
                Convert.ToDecimal(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IValue CreateValue<T>(T value)
        {
            return new NumberValue(value);
        }

        public bool Equals(IValueType other)
        {
            return other is NumberType && other.ValueCode == ValueCode;
        }
    }

    /// <summary>
    /// 数字值实例
    /// </summary>
    public class NumberValue : IValue
    {
        private decimal _value;

        public IValueType ValueType => NumberType.Instance;

        // 强类型构造函数
        public NumberValue(decimal value)
        {
            _value = value;
        }

        // 从其他类型转换的构造函数
        public NumberValue(object value)
        {
            if (value is decimal dec)
            {
                _value = dec;
            }
            else
            {
                try
                {
                    _value = Convert.ToDecimal(value);
                }
                catch
                {
                    _value = 0m;
                }
            }
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public T GetValue<T>()
        {
            if (typeof(T) == typeof(decimal))
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
            if (value is decimal dec)
            {
                _value = dec;
                return true;
            }

            try
            {
                _value = Convert.ToDecimal(value);
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
            if (other is NumberValue numberValue)
            {
                return numberValue.ValueType.Equals(ValueType) && 
                       numberValue.GetValue<decimal>() == _value;
            }
            return false;
        }
    }
} 