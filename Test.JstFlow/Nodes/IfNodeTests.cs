using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using JstFlow.Interface;
using JstFlow.Internal.Nodes;
using JstFlow.Internal.ValueTypes;
using JstFlow.Interface.Models;
using JstFlow.Common;
using JstFlow.Internal.Base;

namespace Test.JstFlow.Nodes
{
    public class IfNodeTests
    {
        private readonly IfNode _ifNode;

        public IfNodeTests()
        {
            _ifNode = new IfNode();
        }

        [Fact]
        public void TestNodeProperties()
        {
            // 测试节点基本属性
            Assert.Equal("判断分支", _ifNode.NodeName);
            Assert.Equal("if", _ifNode.NodeCode);

            // 测试输入参数
            Assert.Equal(2, _ifNode.Inputs.Count);
            Assert.Contains(_ifNode.Inputs, kvp => kvp.Key.Code == "conditionA" && kvp.Key.Name == "条件A");
            Assert.Contains(_ifNode.Inputs, kvp => kvp.Key.Code == "conditionB" && kvp.Key.Name == "条件B");

            // 测试输出参数
            Assert.Single(_ifNode.Outputs);
            Assert.Contains(_ifNode.Outputs, kvp => kvp.Key.Code == "result" && kvp.Key.Name == "结果");
            Assert.Equal(BoolType.Instance, _ifNode.Outputs[new Label("result", "结果")]);

            // 测试输出动作
            Assert.Equal(2, _ifNode.OutputActions.Count);
            Assert.Contains(_ifNode.OutputActions, action => action.Code == "true" && action.Name == "真");
            Assert.Contains(_ifNode.OutputActions, action => action.Code == "false" && action.Name == "假");
        }

        [Fact]
        public async Task TestExecute_EqualBoolValues_ReturnsTrue()
        {
            // 准备测试数据
            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("conditionA", "条件A"), new BoolValue(true) },
                { new Label("conditionB", "条件B"), new BoolValue(true) }
            };

            // 执行测试
            var result = await _ifNode.Execute(inputs);

            // 验证结果
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("true", result.Data.ActionToExecute.Code);
            Assert.Equal("真", result.Data.ActionToExecute.Name);
            Assert.Single(result.Data.Outputs);
            Assert.True(result.Data.Outputs[new Label("result", "结果")].GetValue<bool>());
        }

        [Fact]
        public async Task TestExecute_NotEqualBoolValues_ReturnsFalse()
        {
            // 准备测试数据
            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("conditionA", "条件A"), new BoolValue(true) },
                { new Label("conditionB", "条件B"), new BoolValue(false) }
            };

            // 执行测试
            var result = await _ifNode.Execute(inputs);

            // 验证结果
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("false", result.Data.ActionToExecute.Code);
            Assert.Equal("假", result.Data.ActionToExecute.Name);
            Assert.Single(result.Data.Outputs);
            Assert.False(result.Data.Outputs[new Label("result", "结果")].GetValue<bool>());
        }

        [Fact]
        public async Task TestExecute_EqualStringValues_ReturnsTrue()
        {
            // 准备测试数据
            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("conditionA", "条件A"), new StringValue("hello") },
                { new Label("conditionB", "条件B"), new StringValue("hello") }
            };

            // 执行测试
            var result = await _ifNode.Execute(inputs);

            // 验证结果
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("true", result.Data.ActionToExecute.Code);
            Assert.True(result.Data.Outputs[new Label("result", "结果")].GetValue<bool>());
        }

        [Fact]
        public async Task TestExecute_NotEqualStringValues_ReturnsFalse()
        {
            // 准备测试数据
            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("conditionA", "条件A"), new StringValue("hello") },
                { new Label("conditionB", "条件B"), new StringValue("world") }
            };

            // 执行测试
            var result = await _ifNode.Execute(inputs);

            // 验证结果
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("false", result.Data.ActionToExecute.Code);
            Assert.False(result.Data.Outputs[new Label("result", "结果")].GetValue<bool>());
        }

        [Fact]
        public async Task TestExecute_EqualNumberValues_ReturnsTrue()
        {
            // 准备测试数据
            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("conditionA", "条件A"), new NumberValue(42.5m) },
                { new Label("conditionB", "条件B"), new NumberValue(42.5m) }
            };

            // 执行测试
            var result = await _ifNode.Execute(inputs);

            // 验证结果
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("true", result.Data.ActionToExecute.Code);
            Assert.True(result.Data.Outputs[new Label("result", "结果")].GetValue<bool>());
        }

        [Fact]
        public async Task TestExecute_NotEqualNumberValues_ReturnsFalse()
        {
            // 准备测试数据
            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("conditionA", "条件A"), new NumberValue(42.5m) },
                { new Label("conditionB", "条件B"), new NumberValue(100.0m) }
            };

            // 执行测试
            var result = await _ifNode.Execute(inputs);

            // 验证结果
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("false", result.Data.ActionToExecute.Code);
            Assert.False(result.Data.Outputs[new Label("result", "结果")].GetValue<bool>());
        }

        [Fact]
        public async Task TestExecute_EqualListValues_ReturnsTrue()
        {
            // 准备测试数据
            var list1 = new List<object> { 1, 2, 3 };
            var list2 = new List<object> { 1, 2, 3 };
            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("conditionA", "条件A"), new ListValue(list1) },
                { new Label("conditionB", "条件B"), new ListValue(list2) }
            };

            // 执行测试
            var result = await _ifNode.Execute(inputs);

            // 验证结果
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("true", result.Data.ActionToExecute.Code);
            Assert.True(result.Data.Outputs[new Label("result", "结果")].GetValue<bool>());
        }

        [Fact]
        public async Task TestExecute_NotEqualListValues_ReturnsFalse()
        {
            // 准备测试数据
            var list1 = new List<object> { 1, 2, 3 };
            var list2 = new List<object> { 1, 2, 4 };
            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("conditionA", "条件A"), new ListValue(list1) },
                { new Label("conditionB", "条件B"), new ListValue(list2) }
            };

            // 执行测试
            var result = await _ifNode.Execute(inputs);

            // 验证结果
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("false", result.Data.ActionToExecute.Code);
            Assert.False(result.Data.Outputs[new Label("result", "结果")].GetValue<bool>());
        }

        [Fact]
        public void TestIsValid_ValidInputs_ReturnsSuccess()
        {
            // 准备测试数据
            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("conditionA", "条件A"), new BoolValue(true) },
                { new Label("conditionB", "条件B"), new BoolValue(false) }
            };

            // 执行测试
            var result = _ifNode.IsValid(inputs);

            // 验证结果
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void TestIsValid_IncorrectParameterCount_ReturnsFailure()
        {
            // 准备测试数据 - 参数数量不正确
            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("conditionA", "条件A"), new BoolValue(true) }
                // 缺少第二个参数
            };

            // 执行测试
            var result = _ifNode.IsValid(inputs);

            // 验证结果
            Assert.False(result.IsSuccess);
            Assert.Contains("输入参数数量不正确", result.Message);
        }

        [Fact]
        public void TestIsValid_DifferentTypes_ReturnsFailure()
        {
            // 准备测试数据 - 不同类型
            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("conditionA", "条件A"), new BoolValue(true) },
                { new Label("conditionB", "条件B"), new StringValue("test") }
            };

            // 执行测试
            var result = _ifNode.IsValid(inputs);

            // 验证结果
            Assert.False(result.IsSuccess);
            Assert.Contains("输入参数类型不一致", result.Message);
        }

        [Fact]
        public void TestIsValid_SameTypes_ReturnsSuccess()
        {
            // 准备测试数据 - 相同类型
            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("conditionA", "条件A"), new StringValue("hello") },
                { new Label("conditionB", "条件B"), new StringValue("world") }
            };

            // 执行测试
            var result = _ifNode.IsValid(inputs);

            // 验证结果
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task TestExecute_EdgeCases()
        {
            // 测试边界情况：空字符串
            var inputs1 = new Dictionary<Label, IValue>
            {
                { new Label("conditionA", "条件A"), new StringValue("") },
                { new Label("conditionB", "条件B"), new StringValue("") }
            };
            var result1 = await _ifNode.Execute(inputs1);
            Assert.True(result1.IsSuccess);
            Assert.True(result1.Data.Outputs[new Label("result", "结果")].GetValue<bool>());

            // 测试边界情况：零值数字
            var inputs2 = new Dictionary<Label, IValue>
            {
                { new Label("conditionA", "条件A"), new NumberValue(0m) },
                { new Label("conditionB", "条件B"), new NumberValue(0m) }
            };
            var result2 = await _ifNode.Execute(inputs2);
            Assert.True(result2.IsSuccess);
            Assert.True(result2.Data.Outputs[new Label("result", "结果")].GetValue<bool>());

            // 测试边界情况：空列表
            var inputs3 = new Dictionary<Label, IValue>
            {
                { new Label("conditionA", "条件A"), new ListValue(new List<object>()) },
                { new Label("conditionB", "条件B"), new ListValue(new List<object>()) }
            };
            var result3 = await _ifNode.Execute(inputs3);
            Assert.True(result3.IsSuccess);
            Assert.True(result3.Data.Outputs[new Label("result", "结果")].GetValue<bool>());
        }

        [Fact]
        public async Task TestExecute_ComplexDataTypes()
        {
            // 测试复杂数据类型：嵌套列表
            var nestedList1 = new List<object> { new List<object> { 1, 2 }, 3 };
            var nestedList2 = new List<object> { new List<object> { 1, 2 }, 3 };
            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("conditionA", "条件A"), new ListValue(nestedList1) },
                { new Label("conditionB", "条件B"), new ListValue(nestedList2) }
            };

            var result = await _ifNode.Execute(inputs);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data.Outputs[new Label("result", "结果")].GetValue<bool>());
        }
    }
}