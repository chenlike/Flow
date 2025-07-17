using JstFlow.Core;
using JstFlow.External.Nodes;
using JstFlow.External.Expressions;
using JstFlow.Attributes;
using JstFlow.Core.Metas;
using JstFlow.Core.NodeMeta;
using JstFlow.External;
using Xunit;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Test.JstFlow.Node
{
    public class NodeFactoryTests
    {
        // 测试用的普通节点类
        [FlowNode("测试节点")]
        private class TestNode : FlowBaseNode
        {
            [FlowInput("输入字段1")]
            public string Input1 { get; set; }

            [FlowInput("输入字段2", Required = true)]
            public int Input2 { get; set; }

            [FlowOutput("输出字段1")]
            public string Output1 { get; set; }

            [FlowSignal("测试信号")]
            public FlowOutEvent TestSignal()
            {
                return Emit(() => new FlowEndpoint());
            }
        }

        // 测试用的表达式类
        [FlowExpr("测试表达式")]
        private class TestExpression : FlowExpression<string>
        {
            [FlowInput("表达式输入")]
            public string Input { get; set; }

            public override string Evaluate()
            {
                return Input;
            }
        }

        // 测试用的无效节点类（不继承FlowBaseNode）
        private class InvalidNode
        {
            [FlowInput("输入字段")]
            public string Input { get; set; }
        }

        // 测试用的节点类（同时作为输入和输出）
        [FlowNode("冲突节点")]
        private class ConflictNode : FlowBaseNode
        {
            [FlowInput("冲突字段")]
            [FlowOutput("冲突字段")]
            public string ConflictField { get; set; }
        }

        // 测试用的节点类（有初始值的输入字段）
        [FlowNode("初始值节点")]
        private class InitValueNode : FlowBaseNode
        {
            [FlowInput("有初始值的字段", Required = true)]
            public int RequiredFieldWithInitValue { get; set; } = 42;

            [FlowInput("可选字段")]
            public string OptionalFieldWithInitValue { get; set; } = "默认值";

            [FlowInput("必填字段", Required = true)]
            public string RequiredFieldWithoutInitValue { get; set; }

            [FlowSignal("测试信号")]
            public void TestSignal()
            {
                // 空实现
            }
        }

        // 测试用的节点类（无效的信号方法）
        [FlowNode("无效信号节点")]
        private class InvalidSignalNode : FlowBaseNode
        {
            [FlowSignal("无效信号")]
            public string InvalidSignal()
            {
                return "invalid";
            }
        }

        // 测试用的节点类（带有FlowEvent属性）
        [FlowNode("事件节点")]
        private class EventNode : FlowBaseNode
        {
            [FlowInput("输入字段")]
            public string Input { get; set; }

            [FlowEvent("事件1")]
            public FlowEndpoint Event1 { get; set; }

            [FlowEvent("事件2")]
            public FlowEndpoint Event2 { get; set; }

            [FlowSignal("触发事件1")]
            public FlowOutEvent TriggerEvent1()
            {
                return Emit(() => Event1);
            }

            [FlowSignal("触发事件2")]
            public FlowOutEvent TriggerEvent2()
            {
                return Emit(() => Event2);
            }
        }

        // 测试用的节点类（非FlowEndpoint类型的FlowEvent属性）
        [FlowNode("无效事件节点")]
        private class InvalidEventNode : FlowBaseNode
        {
            [FlowEvent("无效事件")]
            public string InvalidEvent { get; set; }
        }

        // 测试用的节点类（没有FlowEvent属性的FlowEndpoint属性）
        [FlowNode("普通端点节点")]
        private class NormalEndpointNode : FlowBaseNode
        {
            [FlowInput("输入字段")]
            public string Input { get; set; }

            public FlowEndpoint NormalEndpoint { get; set; }
        }

        [Fact]
        public void CreateNodeInfo_WithValidNode_ShouldReturnSuccess()
        {
            // Arrange
            var nodeType = typeof(TestNode);

            // Act
            var result = FlowNodeBuilder.Build(nodeType);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(nodeType, result.Data.NodeImplType);
            Assert.Equal(nodeType.FullName, result.Data.NodeImplTypeFullName);
            Assert.Equal(NodeKind.Node, result.Data.Kind);
            Assert.Equal("测试节点", result.Data.Label.DisplayName);
            Assert.Equal("TestNode", result.Data.Label.Code);
        }

        [Fact]
        public void CreateNodeInfo_WithExpression_ShouldReturnExpressionKind()
        {
            // Arrange
            var expressionType = typeof(TestExpression);

            // Act
            var result = FlowNodeBuilder.Build(expressionType);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(NodeKind.Expression, result.Data.Kind);
            Assert.Equal("测试表达式", result.Data.Label.DisplayName);
            Assert.Equal("TestExpression", result.Data.Label.Code);
        }

        [Fact]
        public void CreateNodeInfo_WithStartNode_ShouldReturnStartNodeKind()
        {
            // Arrange
            var startNodeType = typeof(StartNode);

            // Act
            var result = FlowNodeBuilder.Build(startNodeType);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(NodeKind.StartNode, result.Data.Kind);
        }

        [Fact]
        public void CreateNodeInfo_WithInvalidNode_ShouldReturnFailure()
        {
            // Arrange
            var result = FlowNodeBuilder.Build(typeof(InvalidNode));
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("类型 InvalidNode 必须继承 FlowBaseNode", result.Message);
        }

        [Fact]
        public void CreateNodeInfo_WithInputFields_ShouldPopulateInputFields()
        {
            // Arrange
            var nodeType = typeof(TestNode);

            // Act
            var result = FlowNodeBuilder.Build(nodeType);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Data.InputFields.Count);
            
            var input1 = result.Data.InputFields.Find(f => f.Label.Code == "Input1");
            Assert.NotNull(input1);
            Assert.Equal("输入字段1", input1.Label.DisplayName);
            Assert.Equal("String", input1.Type);
            Assert.False(input1.Required);

            var input2 = result.Data.InputFields.Find(f => f.Label.Code == "Input2");
            Assert.NotNull(input2);
            Assert.Equal("输入字段2", input2.Label.DisplayName);
            Assert.Equal("Int32", input2.Type);
            Assert.True(input2.Required);
        }

        [Fact]
        public void CreateNodeInfo_WithOutputFields_ShouldPopulateOutputFields()
        {
            // Arrange
            var nodeType = typeof(TestNode);

            // Act
            var result = FlowNodeBuilder.Build(nodeType);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Data.OutputFields.Count);
            
            var output1 = result.Data.OutputFields.Find(f => f.Label.Code == "Output1");
            Assert.NotNull(output1);
            Assert.Equal("输出字段1", output1.Label.DisplayName);
            Assert.Equal("String", output1.Type);
        }

        [Fact]
        public void CreateNodeInfo_WithSignals_ShouldPopulateSignals()
        {
            // Arrange
            var nodeType = typeof(TestNode);

            // Act
            var result = FlowNodeBuilder.Build(nodeType);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Data.Signals.Count);
            
            var signal = result.Data.Signals.Find(s => s.Label.Code == "TestSignal");
            Assert.NotNull(signal);
            Assert.Equal("测试信号", signal.Label.DisplayName);
        }

        [Fact]
        public void CreateNodeInfo_WithConflictField_ShouldReturnFailure()
        {
            // Arrange
            var result = FlowNodeBuilder.Build(typeof(ConflictNode));
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("属性 ConflictField 不能同时作为输入和输出", result.Message);
        }

        [Fact]
        public void CreateNodeInfo_WithInvalidSignal_ShouldReturnFailure()
        {
            // Arrange
            var result = FlowNodeBuilder.Build(typeof(InvalidSignalNode));
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("方法 InvalidSignal 的返回类型 String 非法，必须为 void 或 FlowOutEvent ,IEnumable<FlowOutEvent>等有效类型", result.Message);
        }

        [Fact]
        public void CreateNodeInfo_WithNodeWithoutAttribute_ShouldUseTypeName()
        {
            // Arrange
            var nodeType = typeof(FlowBaseNode);

            // Act
            var result = FlowNodeBuilder.Build(nodeType);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("FlowBaseNode", result.Data.Label.DisplayName);
            Assert.Equal("FlowBaseNode", result.Data.Label.Code);
        }

        [Fact]
        public void CreateNodeInfo_ShouldGenerateUniqueId()
        {
            // Arrange
            var nodeType = typeof(TestNode);

            // Act
            var result1 = FlowNodeBuilder.Build(nodeType);
            var result2 = FlowNodeBuilder.Build(nodeType);

            // Assert
            Assert.True(result1.IsSuccess);
            Assert.True(result2.IsSuccess);
            Assert.NotEqual(result1.Data.Id, result2.Data.Id);
        }

        [Fact]
        public void CreateNodeInfo_WithNullType_ShouldReturnFailure()
        {
            // Arrange
            Type nullType = null;

            // Act
            var result = FlowNodeBuilder.Build(nullType);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("节点类型不能为null", result.Message);
        }

        [Fact]
        public void CreateNodeInfo_WithExpression_ShouldHaveCorrectInputFields()
        {
            // Arrange
            var expressionType = typeof(AddExpression<int>);

            // Act
            var result = FlowNodeBuilder.Build(expressionType);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Data.InputFields.Count);
            
            var leftField = result.Data.InputFields.Find(f => f.Label.Code == "Left");
            Assert.NotNull(leftField);
            Assert.Equal("左操作数", leftField.Label.DisplayName);
            Assert.Equal("Int32", leftField.Type);

            var rightField = result.Data.InputFields.Find(f => f.Label.Code == "Right");
            Assert.NotNull(rightField);
            Assert.Equal("右操作数", rightField.Label.DisplayName);
            Assert.Equal("Int32", rightField.Type);
        }

        [Fact]
        public void CreateNodeInfo_WithConcatExpression_ShouldHaveCorrectInputFields()
        {
            // Arrange
            var expressionType = typeof(ConcatExpression);

            // Act
            var result = FlowNodeBuilder.Build(expressionType);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Data.InputFields.Count);
            
            var stringListField = result.Data.InputFields.Find(f => f.Label.Code == "StringList");
            Assert.NotNull(stringListField);
            Assert.Equal("文本列表", stringListField.Label.DisplayName);
            Assert.Equal("List`1", stringListField.Type);
        }

        [Fact]
        public void CreateNodeInfo_WithFlowEventProperties_ShouldPopulateEmits()
        {
            // Arrange
            var nodeType = typeof(EventNode);

            // Act
            var result = FlowNodeBuilder.Build(nodeType);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Data.Emits.Count);
            
            var event1 = result.Data.Emits.Find(e => e.Label.Code == "Event1");
            Assert.NotNull(event1);
            Assert.Equal("事件1", event1.Label.DisplayName);
            Assert.Equal(typeof(FlowEndpoint), event1.PropertyInfo.PropertyType);

            var event2 = result.Data.Emits.Find(e => e.Label.Code == "Event2");
            Assert.NotNull(event2);
            Assert.Equal("事件2", event2.Label.DisplayName);
            Assert.Equal(typeof(FlowEndpoint), event2.PropertyInfo.PropertyType);
        }

        [Fact]
        public void CreateNodeInfo_WithNonFlowEndpointFlowEvent_ShouldNotAddToEmits()
        {
            // Arrange
            var nodeType = typeof(InvalidEventNode);

            // Act
            var result = FlowNodeBuilder.Build(nodeType);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(0, result.Data.Emits.Count);
        }

        [Fact]
        public void CreateNodeInfo_WithNormalFlowEndpoint_ShouldNotAddToEmits()
        {
            // Arrange
            var nodeType = typeof(NormalEndpointNode);

            // Act
            var result = FlowNodeBuilder.Build(nodeType);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(0, result.Data.Emits.Count);
        }

        [Fact]
        public void CreateNodeInfo_WithForNode_ShouldHaveCorrectEmits()
        {
            // Arrange
            var nodeType = typeof(ForNode);

            // Act
            var result = FlowNodeBuilder.Build(nodeType);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Data.Emits.Count);
            
            var loopBody = result.Data.Emits.Find(e => e.Label.Code == "LoopBody");
            Assert.NotNull(loopBody);
            Assert.Equal("循环体", loopBody.Label.DisplayName);
            Assert.Equal(typeof(FlowEndpoint), loopBody.PropertyInfo.PropertyType);

            var loopCompleted = result.Data.Emits.Find(e => e.Label.Code == "LoopCompleted");
            Assert.NotNull(loopCompleted);
            Assert.Equal("循环完成", loopCompleted.Label.DisplayName);
            Assert.Equal(typeof(FlowEndpoint), loopCompleted.PropertyInfo.PropertyType);
        }

        [Fact]
        public void CreateNodeInfo_WithStartNode_ShouldHaveCorrectEmits()
        {
            // Arrange
            var nodeType = typeof(StartNode);

            // Act
            var result = FlowNodeBuilder.Build(nodeType);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Data.Emits.Count);
            
            var startEvent = result.Data.Emits.Find(e => e.Label.Code == "Start");
            Assert.NotNull(startEvent);
            Assert.Equal("开始执行", startEvent.Label.DisplayName);
            Assert.Equal(typeof(FlowEndpoint), startEvent.PropertyInfo.PropertyType);
        }

        [Fact]
        public void CreateNodeInfo_WithIfElseNode_ShouldHaveCorrectEmits()
        {
            // Arrange
            var nodeType = typeof(IfElseNode);

            // Act
            var result = FlowNodeBuilder.Build(nodeType);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Data.Emits.Count);
            
            var trueBranch = result.Data.Emits.Find(e => e.Label.Code == "TrueBranch");
            Assert.NotNull(trueBranch);
            Assert.Equal("真", trueBranch.Label.DisplayName);
            Assert.Equal(typeof(FlowEndpoint), trueBranch.PropertyInfo.PropertyType);

            var falseBranch = result.Data.Emits.Find(e => e.Label.Code == "FalseBranch");
            Assert.NotNull(falseBranch);
            Assert.Equal("假", falseBranch.Label.DisplayName);
            Assert.Equal(typeof(FlowEndpoint), falseBranch.PropertyInfo.PropertyType);
        }

        [Fact]
        public void CreateNodeInfo_WithDebugLogNode_ShouldHaveCorrectEmits()
        {
            // Arrange
            var nodeType = typeof(DebugLogNode);

            // Act
            var result = FlowNodeBuilder.Build(nodeType);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Data.Emits.Count);
            
            var nextEvent = result.Data.Emits.Find(e => e.Label.Code == "Next");
            Assert.NotNull(nextEvent);
            Assert.Equal("下一步", nextEvent.Label.DisplayName);
            Assert.Equal(typeof(FlowEndpoint), nextEvent.PropertyInfo.PropertyType);
        }

        [Fact]
        public void CreateNodeInfo_WithEmptyEmits_ShouldHaveEmptyEmitsList()
        {
            // Arrange
            var nodeType = typeof(TestNode);

            // Act
            var result = FlowNodeBuilder.Build(nodeType);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(0, result.Data.Emits.Count);
        }

        // 测试用的节点类（没有标签的FlowEvent属性）
        [FlowNode("测试节点")]
        private class TestNodeWithUnlabeledEvent : FlowBaseNode
        {
            [FlowEvent]
            public FlowEndpoint UnlabeledEvent { get; set; }
        }

        [Fact]
        public void CreateNodeInfo_WithFlowEventWithoutLabel_ShouldUsePropertyName()
        {
            // Arrange
            var nodeType = typeof(TestNodeWithUnlabeledEvent);

            // Act
            var result = FlowNodeBuilder.Build(nodeType);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Data.Emits.Count);
            
            var emit = result.Data.Emits[0];
            Assert.Equal("UnlabeledEvent", emit.Label.DisplayName);
            Assert.Equal("UnlabeledEvent", emit.Label.Code);
        }

        [Fact]
        public void CreateNodeInfo_WithInitValue_ShouldSetInitValue()
        {
            // Arrange
            var nodeType = typeof(InitValueNode);

            // Act
            var result = FlowNodeBuilder.Build(nodeType);

            // Assert
            Assert.True(result.IsSuccess);
            
            var requiredFieldWithInitValue = result.Data.InputFields.Find(f => f.Label.Code == "RequiredFieldWithInitValue");
            Assert.NotNull(requiredFieldWithInitValue);
            Assert.True(requiredFieldWithInitValue.Required);
            Assert.Equal(42, requiredFieldWithInitValue.InitValue);

            var optionalFieldWithInitValue = result.Data.InputFields.Find(f => f.Label.Code == "OptionalFieldWithInitValue");
            Assert.NotNull(optionalFieldWithInitValue);
            Assert.False(optionalFieldWithInitValue.Required);
            Assert.Equal("默认值", optionalFieldWithInitValue.InitValue);

            var requiredFieldWithoutInitValue = result.Data.InputFields.Find(f => f.Label.Code == "RequiredFieldWithoutInitValue");
            Assert.NotNull(requiredFieldWithoutInitValue);
            Assert.True(requiredFieldWithoutInitValue.Required);
            Assert.Null(requiredFieldWithoutInitValue.InitValue);
        }

        [Fact]
        public void CreateNodeInfo_WithInitValueAndSetValue_ShouldPrioritizeSetValue()
        {
            // Arrange
            var nodeType = typeof(InitValueNode);
            var initValues = new Dictionary<string, object>
            {
                { "RequiredFieldWithInitValue", 100 }
            };

            // Act
            var result = FlowNodeBuilder.Build(nodeType, initValues);

            // Assert
            Assert.True(result.IsSuccess);
            
            var requiredFieldWithInitValue = result.Data.InputFields.Find(f => f.Label.Code == "RequiredFieldWithInitValue");
            Assert.NotNull(requiredFieldWithInitValue);
            Assert.Equal(100, requiredFieldWithInitValue.InitValue); // 应该使用 SetValue 的值，而不是属性上的 InitValue
        }
    }
} 