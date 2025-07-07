using JstFlow.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace JstFlow.Internal.ValueTypes
{
    public class ArrayValueType : IValue
    {
        private object[] _value;

        public string ValueName => "数组";
        public string ValueCode => "array";

        public override string ToString()
        {
            if (_value == null)
                return "[]";
            return "[" + string.Join(", ", _value.Select(v => v?.ToString() ?? "null")) + "]";
        }

        public T GetValue<T>()
        {
            if (_value == null)
                return default;

            if (typeof(T) == typeof(object[]))
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
            if (value is object[] arr)
            {
                _value = arr;
                return true;
            }

            if (value is Array array)
            {
                _value = new object[array.Length];
                for (int i = 0; i < array.Length; i++)
                {
                    _value[i] = array.GetValue(i);
                }
                return true;
            }

            try
            {
                // 尝试将单个值转换为数组
                _value = new object[] { value };
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IsValid<T>(T value)
        {
            if (value is object[] || value is Array)
                return true;

            // 单个值也可以作为只有一个元素的数组
            return true;
        }

        public bool Equals(IValue other)
        {
            if (other is ArrayValueType arrayValue)
            {
                return arrayValue.ValueCode == ValueCode;
            }
            return false;
        }
    }
} 