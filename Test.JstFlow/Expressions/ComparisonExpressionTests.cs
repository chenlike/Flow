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
    public class ComparisonExpressionTests
    {
        [Fact]
        public void TestGreaterThanExpressionProperties()
        {
            var expression = new GreaterThanExpression();
            Assert.Equal("大于比较", expression.ExpressionName);
            Assert.Equal(">", expression.ExpressionCode);
            Assert.NotNull(expression.ReturnType);
            Assert.NotNull(expression.Inputs);
            Assert.Equal(2, expression.Inputs.Count);
        }

        [Fact]
        public void TestLessThanExpressionProperties()
        {
            var expression = new LessThanExpression();
            Assert.Equal("小于比较", expression.ExpressionName);
            Assert.Equal("<", expression.ExpressionCode);
            Assert.NotNull(expression.ReturnType);
            Assert.NotNull(expression.Inputs);
            Assert.Equal(2, expression.Inputs.Count);
        }

        [Fact]
        public void TestGreaterThanOrEqualExpressionProperties()
        {
            var expression = new GreaterThanOrEqualExpression();
            Assert.Equal("大于等于比较", expression.ExpressionName);
            Assert.Equal(">=", expression.ExpressionCode);
            Assert.NotNull(expression.ReturnType);
            Assert.NotNull(expression.Inputs);
            Assert.Equal(2, expression.Inputs.Count);
        }

        [Fact]
        public void TestLessThanOrEqualExpressionProperties()
        {
            var expression = new LessThanOrEqualExpression();
            Assert.Equal("小于等于比较", expression.ExpressionName);
            Assert.Equal("<=", expression.ExpressionCode);
            Assert.NotNull(expression.ReturnType);
            Assert.NotNull(expression.Inputs);
            Assert.Equal(2, expression.Inputs.Count);
        }

        [Fact]
        public async Task TestGreaterThanExpression()
        {
            var expression = new GreaterThanExpression();
            
            // 测试 5 > 3
            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("left", "左操作数"), new NumberValue(5) },
                { new Label("right", "右操作数"), new NumberValue(3) }
            };

            var result = await expression.Evaluate(inputs);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("result", result.Data.Code);
            Assert.Equal("比较结果", result.Data.Name);

            var resultValue = result.Data.Value;
            Assert.IsType<BoolValue>(resultValue);
            Assert.True(resultValue.GetValue<bool>());

            // 测试 3 > 5
            inputs = new Dictionary<Label, IValue>
            {
                { new Label("left", "左操作数"), new NumberValue(3) },
                { new Label("right", "右操作数"), new NumberValue(5) }
            };

            result = await expression.Evaluate(inputs);
            Assert.True(result.Success);
            resultValue = result.Data.Value;
            Assert.IsType<BoolValue>(resultValue);
            Assert.False(resultValue.GetValue<bool>());
        }

        [Fact]
        public async Task TestLessThanExpression()
        {
            var expression = new LessThanExpression();
            
            // 测试 3 < 5
            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("left", "左操作数"), new NumberValue(3) },
                { new Label("right", "右操作数"), new NumberValue(5) }
            };

            var result = await expression.Evaluate(inputs);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("result", result.Data.Code);
            Assert.Equal("比较结果", result.Data.Name);

            var resultValue = result.Data.Value;
            Assert.IsType<BoolValue>(resultValue);
            Assert.True(resultValue.GetValue<bool>());

            // 测试 5 < 3
            inputs = new Dictionary<Label, IValue>
            {
                { new Label("left", "左操作数"), new NumberValue(5) },
                { new Label("right", "右操作数"), new NumberValue(3) }
            };

            result = await expression.Evaluate(inputs);
            Assert.True(result.Success);
            resultValue = result.Data.Value;
            Assert.IsType<BoolValue>(resultValue);
            Assert.False(resultValue.GetValue<bool>());
        }

        [Fact]
        public async Task TestGreaterThanOrEqualExpression()
        {
            var expression = new GreaterThanOrEqualExpression();
            
            // 测试 5 >= 3
            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("left", "左操作数"), new NumberValue(5) },
                { new Label("right", "右操作数"), new NumberValue(3) }
            };

            var result = await expression.Evaluate(inputs);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("result", result.Data.Code);
            Assert.Equal("比较结果", result.Data.Name);

            var resultValue = result.Data.Value;
            Assert.IsType<BoolValue>(resultValue);
            Assert.True(resultValue.GetValue<bool>());

            // 测试 5 >= 5
            inputs = new Dictionary<Label, IValue>
            {
                { new Label("left", "左操作数"), new NumberValue(5) },
                { new Label("right", "右操作数"), new NumberValue(5) }
            };

            result = await expression.Evaluate(inputs);
            Assert.True(result.Success);
            resultValue = result.Data.Value;
            Assert.IsType<BoolValue>(resultValue);
            Assert.True(resultValue.GetValue<bool>());

            // 测试 3 >= 5
            inputs = new Dictionary<Label, IValue>
            {
                { new Label("left", "左操作数"), new NumberValue(3) },
                { new Label("right", "右操作数"), new NumberValue(5) }
            };

            result = await expression.Evaluate(inputs);
            Assert.True(result.Success);
            resultValue = result.Data.Value;
            Assert.IsType<BoolValue>(resultValue);
            Assert.False(resultValue.GetValue<bool>());
        }

        [Fact]
        public async Task TestLessThanOrEqualExpression()
        {
            var expression = new LessThanOrEqualExpression();
            
            // 测试 3 <= 5
            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("left", "左操作数"), new NumberValue(3) },
                { new Label("right", "右操作数"), new NumberValue(5) }
            };

            var result = await expression.Evaluate(inputs);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("result", result.Data.Code);
            Assert.Equal("比较结果", result.Data.Name);

            var resultValue = result.Data.Value;
            Assert.IsType<BoolValue>(resultValue);
            Assert.True(resultValue.GetValue<bool>());

            // 测试 5 <= 5
            inputs = new Dictionary<Label, IValue>
            {
                { new Label("left", "左操作数"), new NumberValue(5) },
                { new Label("right", "右操作数"), new NumberValue(5) }
            };

            result = await expression.Evaluate(inputs);
            Assert.True(result.Success);
            resultValue = result.Data.Value;
            Assert.IsType<BoolValue>(resultValue);
            Assert.True(resultValue.GetValue<bool>());

            // 测试 5 <= 3
            inputs = new Dictionary<Label, IValue>
            {
                { new Label("left", "左操作数"), new NumberValue(5) },
                { new Label("right", "右操作数"), new NumberValue(3) }
            };

            result = await expression.Evaluate(inputs);
            Assert.True(result.Success);
            resultValue = result.Data.Value;
            Assert.IsType<BoolValue>(resultValue);
            Assert.False(resultValue.GetValue<bool>());
        }

        [Fact]
        public async Task TestInvalidTypeForComparison()
        {
            var expression = new GreaterThanExpression();
            
            // 测试字符串类型（应该失败）
            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("left", "左操作数"), new StringValue("hello") },
                { new Label("right", "右操作数"), new NumberValue(3) }
            };

            var result = await expression.Evaluate(inputs);
            Assert.False(result.Success);
            Assert.Contains("只支持数字类型", result.Message);
        }

        [Fact]
        public async Task TestMissingInputs()
        {
            var expression = new GreaterThanExpression();
            
            // 测试缺少输入参数
            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("left", "左操作数"), new NumberValue(5) }
                // 缺少右操作数
            };

            var result = await expression.Evaluate(inputs);
            Assert.False(result.Success);
            Assert.Contains("缺少必需的输入参数", result.Message);
        }

        [Fact]
        public async Task TestNullInputs()
        {
            var expression = new GreaterThanExpression();
            
            // 测试空输入
            var result = await expression.Evaluate(null);
            Assert.False(result.Success);
            Assert.Contains("输入参数不能为空", result.Message);
        }
    }
} 