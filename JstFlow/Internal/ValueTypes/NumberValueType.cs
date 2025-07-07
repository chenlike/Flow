using JstFlow.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.Internal.ValueTypes
{
    public class NumberValueType : IValue
    {
        private decimal _value;

        public string ValueName => "数字";
        public string ValueCode => "number";

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
            if (value is decimal dec)
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

        public bool Equals(IValue other)
        {
            if (other is NumberValueType numberValue)
            {
                return numberValue.ValueCode == ValueCode;
            }
            return false;
        }
    }
} 