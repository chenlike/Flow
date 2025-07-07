using JstFlow.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.Internal.ValueTypes
{
    public class BoolValueType : IValue
    {
        private bool _value;

        public string ValueName => "布尔值";
        public string ValueCode => "bool";


        public BoolValueType(bool value)
        {
            _value = value;
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
            if (value is bool b)
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

        public bool Equals(IValue other)
        {
            if (other is BoolValueType boolValue)
            {
                return boolValue.ValueCode == ValueCode;
            }
            return false;
        }
    }
} 