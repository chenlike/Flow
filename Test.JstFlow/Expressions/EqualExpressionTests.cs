using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JstFlow.Internal.Expressions;
using JstFlow.Internal.ValueTypes;
using JstFlow.Internal.Base;
using JstFlow.Interface;

namespace Test.JstFlow.Expressions
{
    public class EqualExpressionTests
    {
        private EqualExpression _expression;

        public EqualExpressionTests()
        {
            _expression = new EqualExpression();
        }

        [Fact]
        public void TestExpressionProperties()
        {
            // 验证表达式属性
            Assert.Equal("相等比较", _expression.ExpressionName);
            Assert.Equal("==", _expression.ExpressionCode);
            Assert.NotNull(_expression.ReturnType);
            Assert.NotNull(_expression.Inputs);
            Assert.Equal(2, _expression.Inputs.Count);
        }

        [Fact]
        public async Task TestEqualStringValues()
        {
            // 测试相等的字符串值
            var leftValue = new StringValue("hello");
            var rightValue = new StringValue("hello");

            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("left", "左操作数"), leftValue },
                { new Label("right", "右操作数"), rightValue }
            };

            var result = await _expression.Evaluate(inputs);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("result", result.Data.Code);
            Assert.Equal("比较结果", result.Data.Name);

            var resultValue = result.Data.Value;
            Assert.IsType<BoolValue>(resultValue);
            Assert.True(resultValue.GetValue<bool>());
        }

        [Fact]
        public async Task TestDifferentStringValues()
        {
            // 测试不相等的字符串值
            var leftValue = new StringValue("hello");
            var rightValue = new StringValue("world");

            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("left", "左操作数"), leftValue },
                { new Label("right", "右操作数"), rightValue }
            };

            var result = await _expression.Evaluate(inputs);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("result", result.Data.Code);
            Assert.Equal("比较结果", result.Data.Name);

            var resultValue = result.Data.Value;
            Assert.IsType<BoolValue>(resultValue);
            Assert.False(resultValue.GetValue<bool>());
        }

        [Fact]
        public async Task TestEqualNumberValues()
        {
            // 测试相等的数字值
            var leftValue = new NumberValue(42);
            var rightValue = new NumberValue(42);

            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("left", "左操作数"), leftValue },
                { new Label("right", "右操作数"), rightValue }
            };

            var result = await _expression.Evaluate(inputs);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("result", result.Data.Code);
            Assert.Equal("比较结果", result.Data.Name);

            var resultValue = result.Data.Value;
            Assert.IsType<BoolValue>(resultValue);
            Assert.True(resultValue.GetValue<bool>());
        }

        [Fact]
        public async Task TestDifferentTypes()
        {
            // 测试不同类型的值（字符串和数字）
            var leftValue = new StringValue("42");
            var rightValue = new NumberValue(42);

            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("left", "左操作数"), leftValue },
                { new Label("right", "右操作数"), rightValue }
            };

            var result = await _expression.Evaluate(inputs);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("result", result.Data.Code);
            Assert.Equal("比较结果", result.Data.Name);

            var resultValue = result.Data.Value;
            Assert.IsType<BoolValue>(resultValue);
            // 字符串"42"和数字42的字符串表示应该相等
            Assert.True(resultValue.GetValue<bool>());
        }

        [Fact]
        public async Task TestMissingInputs()
        {
            // 测试缺少输入参数的情况
            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("left", "左操作数"), new StringValue("test") }
                // 缺少右操作数
            };

            var result = await _expression.Evaluate(inputs);

            Assert.False(result.Success);
            Assert.Contains("缺少必需的输入参数", result.Message);
        }

        [Fact]
        public async Task TestNullInputs()
        {
            // 测试空输入的情况
            var result = await _expression.Evaluate(null);

            Assert.False(result.Success);
            Assert.Contains("输入参数不能为空", result.Message);
        }
    }
} 