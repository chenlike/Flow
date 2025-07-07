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
    public class StringExpressionTests
    {
        [Fact]
        public void TestStringIsEmptyExpressionProperties()
        {
            var expression = new StringIsEmptyExpression();
            Assert.Equal("字符串是否为空", expression.ExpressionName);
            Assert.Equal("string.isEmpty", expression.ExpressionCode);
            Assert.NotNull(expression.ReturnType);
            Assert.NotNull(expression.Inputs);
            Assert.Equal(1, expression.Inputs.Count);
        }

        [Fact]
        public void TestStringLengthExpressionProperties()
        {
            var expression = new StringLengthExpression();
            Assert.Equal("字符串长度", expression.ExpressionName);
            Assert.Equal("string.length", expression.ExpressionCode);
            Assert.NotNull(expression.ReturnType);
            Assert.NotNull(expression.Inputs);
            Assert.Equal(1, expression.Inputs.Count);
        }

        [Fact]
        public async Task TestStringIsEmptyExpression()
        {
            var expression = new StringIsEmptyExpression();
            
            // 测试空字符串
            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("input", "输入字符串"), new StringValue("") }
            };

            var result = await expression.Evaluate(inputs);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("result", result.Data.Code);
            Assert.Equal("判断结果", result.Data.Name);

            var resultValue = result.Data.Value;
            Assert.IsType<BoolValue>(resultValue);
            Assert.True(resultValue.GetValue<bool>());

            // 测试非空字符串
            inputs = new Dictionary<Label, IValue>
            {
                { new Label("input", "输入字符串"), new StringValue("hello") }
            };

            result = await expression.Evaluate(inputs);
            Assert.True(result.Success);
            resultValue = result.Data.Value;
            Assert.IsType<BoolValue>(resultValue);
            Assert.False(resultValue.GetValue<bool>());

            // 测试null字符串
            inputs = new Dictionary<Label, IValue>
            {
                { new Label("input", "输入字符串"), new StringValue(null) }
            };

            result = await expression.Evaluate(inputs);
            Assert.True(result.Success);
            resultValue = result.Data.Value;
            Assert.IsType<BoolValue>(resultValue);
            Assert.True(resultValue.GetValue<bool>());
        }

        [Fact]
        public async Task TestStringLengthExpression()
        {
            var expression = new StringLengthExpression();
            
            // 测试普通字符串
            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("input", "输入字符串"), new StringValue("hello") }
            };

            var result = await expression.Evaluate(inputs);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("result", result.Data.Code);
            Assert.Equal("字符串长度", result.Data.Name);

            var resultValue = result.Data.Value;
            Assert.IsType<NumberValue>(resultValue);
            Assert.Equal(5, resultValue.GetValue<int>());

            // 测试空字符串
            inputs = new Dictionary<Label, IValue>
            {
                { new Label("input", "输入字符串"), new StringValue("") }
            };

            result = await expression.Evaluate(inputs);
            Assert.True(result.Success);
            resultValue = result.Data.Value;
            Assert.IsType<NumberValue>(resultValue);
            Assert.Equal(0, resultValue.GetValue<int>());

            // 测试null字符串
            inputs = new Dictionary<Label, IValue>
            {
                { new Label("input", "输入字符串"), new StringValue(null) }
            };

            result = await expression.Evaluate(inputs);
            Assert.True(result.Success);
            resultValue = result.Data.Value;
            Assert.IsType<NumberValue>(resultValue);
            Assert.Equal(0, resultValue.GetValue<int>());

            // 测试长字符串
            inputs = new Dictionary<Label, IValue>
            {
                { new Label("input", "输入字符串"), new StringValue("这是一个很长的字符串，用来测试长度计算功能") }
            };

            result = await expression.Evaluate(inputs);
            Assert.True(result.Success);
            resultValue = result.Data.Value;
            Assert.IsType<NumberValue>(resultValue);
            Assert.Equal(21, resultValue.GetValue<int>());
        }

        [Fact]
        public async Task TestInvalidTypeForStringIsEmpty()
        {
            var expression = new StringIsEmptyExpression();
            
            // 测试数字类型（应该失败）
            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("input", "输入字符串"), new NumberValue(123) }
            };

            var result = await expression.Evaluate(inputs);
            Assert.False(result.Success);
            Assert.Contains("只支持字符串类型", result.Message);
        }

        [Fact]
        public async Task TestInvalidTypeForStringLength()
        {
            var expression = new StringLengthExpression();
            
            // 测试数字类型（应该失败）
            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("input", "输入字符串"), new NumberValue(123) }
            };

            var result = await expression.Evaluate(inputs);
            Assert.False(result.Success);
            Assert.Contains("只支持字符串类型", result.Message);
        }

        [Fact]
        public async Task TestMissingInputsForStringIsEmpty()
        {
            var expression = new StringIsEmptyExpression();
            
            // 测试缺少输入参数
            var inputs = new Dictionary<Label, IValue>();

            var result = await expression.Evaluate(inputs);
            Assert.False(result.Success);
            Assert.Contains("缺少必需的输入参数", result.Message);
        }

        [Fact]
        public async Task TestMissingInputsForStringLength()
        {
            var expression = new StringLengthExpression();
            
            // 测试缺少输入参数
            var inputs = new Dictionary<Label, IValue>();

            var result = await expression.Evaluate(inputs);
            Assert.False(result.Success);
            Assert.Contains("缺少必需的输入参数", result.Message);
        }

        [Fact]
        public async Task TestNullInputsForStringExpressions()
        {
            var isEmptyExpression = new StringIsEmptyExpression();
            var lengthExpression = new StringLengthExpression();
            
            // 测试空输入
            var result = await isEmptyExpression.Evaluate(null);
            Assert.False(result.Success);
            Assert.Contains("输入参数不能为空", result.Message);

            result = await lengthExpression.Evaluate(null);
            Assert.False(result.Success);
            Assert.Contains("输入参数不能为空", result.Message);
        }
    }
} 