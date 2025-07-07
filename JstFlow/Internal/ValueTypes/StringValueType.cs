using JstFlow.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.Internal.ValueTypes
{
    public class StringValueType : IValue
    {
        private string _value;

        public string ValueName => "字符串";
        public string ValueCode => "string";

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
            return value is string str && !string.IsNullOrWhiteSpace(str);
        }

        public bool Equals(IValue other)
        {
            if (other is StringValueType stringValue)
            {
                return stringValue.ValueCode == ValueCode;
            }
            return false;
        }
    }

}
