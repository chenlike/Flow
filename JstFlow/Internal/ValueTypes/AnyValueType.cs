using JstFlow.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.Internal.ValueTypes
{
    public class AnyType : IValueType
    {
        private static readonly AnyType _instance = new AnyType();
        private AnyType() { }

        public static AnyType Instance => _instance;

        public string ValueName => "任意类型";
        public string ValueCode => "any";

        public bool IsValid<T>(T value)
        {
            return true; // 任何值都合法
        }

        public IValue CreateValue<T>(T value)
        {
            throw new ArgumentException("AnyType.CreateValue 仅作为类型使用");
        }

        public bool Equals(IValueType other)
        {
            return other is AnyType;
        }
    }
}
