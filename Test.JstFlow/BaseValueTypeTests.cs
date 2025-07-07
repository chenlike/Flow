using System;
using Xunit;
using JstFlow.Interface;
using JstFlow.Internal.ValueTypes;
using System.Collections.Generic;

namespace Test.JstFlow
{
    public class BaseValueTypeTests
    {
        [Fact]
        public void TestValueTypeSingleton()
        {
            // 验证单例模式
            var boolType1 = BoolType.Instance;
            var boolType2 = BoolType.Instance;
            Assert.Same(boolType1, boolType2);
            
            var stringType1 = StringType.Instance;
            var stringType2 = StringType.Instance;
            Assert.Same(stringType1, stringType2);
        }

        [Fact]
        public void TestValueCreation()
        {
            // 测试创建布尔值
            var boolValue = new BoolValue(true);
            Assert.NotNull(boolValue);
            Assert.Equal(BoolType.Instance, boolValue.ValueType);
            Assert.True(boolValue.GetValue<bool>());

            // 测试创建字符串值
            var stringValue = new StringValue("测试");
            Assert.NotNull(stringValue);
            Assert.Equal(StringType.Instance, stringValue.ValueType);
            Assert.Equal("测试", stringValue.GetValue<string>());

            // 测试创建数字值
            var numberValue = new NumberValue(123.45m);
            Assert.NotNull(numberValue);
            Assert.Equal(NumberType.Instance, numberValue.ValueType);
            Assert.Equal(123.45m, numberValue.GetValue<decimal>());
        }

        [Fact]
        public void TestValueTypeFactory()
        {
            // 测试工厂类
            var boolType = ValueTypeFactory.GetValueType("bool");
            Assert.NotNull(boolType);
            Assert.Equal("bool", boolType.ValueCode);

            var stringType = ValueTypeFactory.GetValueType("string");
            Assert.NotNull(stringType);
            Assert.Equal("string", stringType.ValueCode);

            // 测试不存在的类型
            var invalidType = ValueTypeFactory.GetValueType("invalid");
            Assert.Null(invalidType);
        }

        [Fact]
        public void TestValueEquality()
        {
            var value1 = new BoolValue(true);
            var value2 = new BoolValue(true);
            var value3 = new BoolValue(false);

            Assert.True(value1.Equals(value2));
            Assert.False(value1.Equals(value3));
        }

        [Fact]
        public void TestDirectValueCreation()
        {
            // 测试直接创建值实例
            var boolValue = new BoolValue(true);
            var stringValue = new StringValue("Hello");
            var numberValue = new NumberValue(42.0m);
            var listValue = new ListValue(new List<object> { 1, 2, 3 });

            Assert.True(boolValue.GetValue<bool>());
            Assert.Equal("Hello", stringValue.GetValue<string>());
            Assert.Equal(42.0m, numberValue.GetValue<decimal>());
            Assert.Equal(3, listValue.GetValue<List<object>>().Count);
        }

        [Fact]
        public void TestListValueOperations()
        {
            // 测试列表值操作
            var listValue = new ListValue(new List<object> { 1, 2, 3 });
            
            // 测试添加元素
            listValue.Add(4);
            Assert.Equal(4, listValue.Count);
            Assert.Equal(4, listValue.GetValue<List<object>>()[3]);

            // 测试插入元素
            listValue.Insert(1, "inserted");
            Assert.Equal(5, listValue.Count);
            Assert.Equal("inserted", listValue.GetValue<List<object>>()[1]);

            // 测试索引器
            listValue[0] = "modified";
            Assert.Equal("modified", listValue.GetValue<List<object>>()[0]);

            // 测试移除元素
            listValue.RemoveAt(1);
            Assert.Equal(4, listValue.Count);

            // 测试清除
            listValue.Clear();
            Assert.Equal(0, listValue.Count);
        }
    }
}
