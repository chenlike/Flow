using JstFlow.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JstFlow.Internal.ValueTypes
{
    /// <summary>
    /// 列表值类型定义 - 单例模式
    /// </summary>
    public class ListType : IValueType
    {
        private static readonly ListType _instance = new ListType();
        
        // 私有构造函数，防止外部实例化
        private ListType() { }
        
        // 单例实例
        public static ListType Instance => _instance;

        public string ValueName => "列表";
        public string ValueCode => "list";

        public bool IsValid<T>(T value)
        {
            if (value is List<object> || value is IList<object>)
                return true;

            // 单个值也可以作为只有一个元素的列表
            return true;
        }

        public IValue CreateValue<T>(T value)
        {
            return new ListValue(value);
        }

        public bool Equals(IValueType other)
        {
            return other is ListType && other.ValueCode == ValueCode;
        }
    }

    /// <summary>
    /// 列表值实例
    /// </summary>
    public class ListValue : IValue
    {
        private List<object> _value;

        public IValueType ValueType => ListType.Instance;

        // 强类型构造函数
        public ListValue(List<object> value)
        {
            _value = value ?? new List<object>();
        }

        // 从其他类型转换的构造函数
        public ListValue(object value)
        {
            if (value is List<object> list)
            {
                _value = list;
            }
            else if (value is IList<object> iList)
            {
                _value = new List<object>(iList);
            }
            else if (value is Array array)
            {
                _value = new List<object>();
                for (int i = 0; i < array.Length; i++)
                {
                    _value.Add(array.GetValue(i));
                }
            }
            else
            {
                // 尝试将单个值转换为列表
                _value = new List<object> { value };
            }
        }

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

            if (typeof(T) == typeof(List<object>))
                return (T)(object)_value;

            if (typeof(T) == typeof(IList<object>))
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
            if (value is List<object> list)
            {
                _value = list;
                return true;
            }

            if (value is IList<object> iList)
            {
                _value = new List<object>(iList);
                return true;
            }

            if (value is Array array)
            {
                _value = new List<object>();
                for (int i = 0; i < array.Length; i++)
                {
                    _value.Add(array.GetValue(i));
                }
                return true;
            }

            try
            {
                // 尝试将单个值转换为列表
                _value = new List<object> { value };
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
            if (other is ListValue listValue)
            {
                if (!listValue.ValueType.Equals(ValueType))
                    return false;

                var otherList = listValue.GetValue<List<object>>();
                if (_value == null && otherList == null)
                    return true;
                if (_value == null || otherList == null)
                    return false;
                if (_value.Count != otherList.Count)
                    return false;

                for (int i = 0; i < _value.Count; i++)
                {
                    var item1 = _value[i];
                    var item2 = otherList[i];

                    // 如果都是 List<object>，递归比较
                    if (item1 is List<object> l1 && item2 is List<object> l2)
                    {
                        if (!new ListValue(l1).Equals(new ListValue(l2)))
                            return false;
                    }
                    // 其他情况直接用 Equals
                    else if (!object.Equals(item1, item2))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        // 添加列表操作方法
        public void Add(object item)
        {
            _value.Add(item);
        }

        public void Insert(int index, object item)
        {
            _value.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _value.RemoveAt(index);
        }

        public bool Remove(object item)
        {
            return _value.Remove(item);
        }

        public void Clear()
        {
            _value.Clear();
        }

        public int Count => _value.Count;

        public object this[int index]
        {
            get => _value[index];
            set => _value[index] = value;
        }
    }
} 