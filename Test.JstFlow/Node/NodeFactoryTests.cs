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
    }
} 