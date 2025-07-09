using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using JstFlow.Internal;
using JstFlow.Internal.NodeMeta;
using JstFlow.Attributes;
using JstFlow.Common;
using JstFlow.Internal.Metas;

namespace Test.JstFlow.Node
{
    public class NodeFactoryTests
    {
        #region 测试节点类定义

        [FlowNode("简单测试节点")]
        public class SimpleTestNode
        {
            [Input("输入名称")]
            public string Name { get; set; }

            [Output("输出结果")]
            public string Result { get; set; }

            [Signal("测试信号")]
            public void TestSignal() { }

            [Emit("测试事件")]
            public event Action TestEvent;
        }

        [FlowNode]
        public class NodeWithoutLabel
        {
            [Input]
            public string Input1 { get; set; }

            [Output]
            public string Output1 { get; set; }
        }

        [FlowNode("复杂测试节点")]
        public class ComplexTestNode
        {
            [Input("必填输入", Required = true)]
            public string RequiredInput { get; set; }

            [Input("可选输入")]
            public int OptionalInput { get; set; }

            [Input("小数输入")]
            public decimal DecimalInput { get; set; }

            [Output("字符串输出")]
            public string StringOutput { get; set; }

            [Output("数字输出")]
            public int NumberOutput { get; set; }

            [Signal("无参数信号")]
            public void SignalWithoutParams() { }

            [Signal("有参数信号")]
            public void SignalWithParams(string param1, int param2) { }

            [Emit("简单事件")]
            public event Action SimpleEvent;

            [Emit("带参数事件")]
            public event Action<string, int> ComplexEvent;
        }

        public class NodeWithoutAttributes
        {
            public string NormalProperty { get; set; }

            public void NormalMethod() { }

            public event Action NormalEvent;
        }

        [FlowExpression("数学表达式")]
        public class MathExpression : FlowExpression
        {
            [Input("左操作数")]
            public int Left { get; set; }

            [Input("右操作数")]
            public int Right { get; set; }

            [Output("计算结果")]
            public int Result { get; set; }

            public override void Evaluate()
            {
                Result = Left + Right;
            }
        }

        public class StringExpression : FlowExpression
        {
            [Input("前缀")]
            public string Prefix { get; set; }

            [Input("后缀")]
            public string Suffix { get; set; }

            [Output("连接结果")]
            public string Result { get; set; }

            public override void Evaluate()
            {
                Result = $"{Prefix}{Suffix}";
            }
        }

        #endregion

        [Fact]
        public void CreateNodeInfo_WithSimpleNode_ShouldCreateCorrectNodeInfo()
        {
            // Arrange
            var nodeType = typeof(SimpleTestNode);

            // Act
            var nodeInfo = NodeFactory.CreateNodeInfo(nodeType);

            // Assert
            Assert.NotNull(nodeInfo);
            Assert.Equal("SimpleTestNode", nodeInfo.Name.Code);
            Assert.Equal("简单测试节点", nodeInfo.Name.Display);
            Assert.Equal(nodeType, nodeInfo.NodeImplType);
            Assert.NotNull(nodeInfo.InputFields);
            Assert.NotNull(nodeInfo.OutputFields);
            Assert.NotNull(nodeInfo.Signals);
            Assert.NotNull(nodeInfo.Emits);
        }

        [Fact]
        public void CreateNodeInfo_WithSimpleNode_ShouldHaveCorrectInputFields()
        {
            // Arrange
            var nodeType = typeof(SimpleTestNode);

            // Act
            var nodeInfo = NodeFactory.CreateNodeInfo(nodeType);

            // Assert
            Assert.Single(nodeInfo.InputFields);
            var inputField = nodeInfo.InputFields[0];
            Assert.Equal("Name", inputField.Name.Code);
            Assert.Equal("输入名称", inputField.Name.Display);
            Assert.Equal("String", inputField.Type);
            Assert.False(inputField.Required);
            Assert.NotNull(inputField.PropertyInfo);
            Assert.Equal("Name", inputField.PropertyInfo.Name);
        }

        [Fact]
        public void CreateNodeInfo_WithSimpleNode_ShouldHaveCorrectOutputFields()
        {
            // Arrange
            var nodeType = typeof(SimpleTestNode);

            // Act
            var nodeInfo = NodeFactory.CreateNodeInfo(nodeType);

            // Assert
            Assert.Single(nodeInfo.OutputFields);
            var outputField = nodeInfo.OutputFields[0];
            Assert.Equal("Result", outputField.Name.Code);
            Assert.Equal("输出结果", outputField.Name.Display);
            Assert.Equal("String", outputField.Type);
            Assert.NotNull(outputField.PropertyInfo);
            Assert.Equal("Result", outputField.PropertyInfo.Name);
        }

        [Fact]
        public void CreateNodeInfo_WithSimpleNode_ShouldHaveCorrectSignals()
        {
            // Arrange
            var nodeType = typeof(SimpleTestNode);

            // Act
            var nodeInfo = NodeFactory.CreateNodeInfo(nodeType);

            // Assert
            Assert.Single(nodeInfo.Signals);
            var signal = nodeInfo.Signals[0];
            Assert.Equal("TestSignal", signal.Name.Code);
            Assert.Equal("测试信号", signal.Name.Display);
            Assert.NotNull(signal.MethodInfo);
            Assert.Equal("TestSignal", signal.MethodInfo.Name);
        }

        [Fact]
        public void CreateNodeInfo_WithSimpleNode_ShouldHaveCorrectEmits()
        {
            // Arrange
            var nodeType = typeof(SimpleTestNode);

            // Act
            var nodeInfo = NodeFactory.CreateNodeInfo(nodeType);

            // Assert
            Assert.Single(nodeInfo.Emits);
            var emit = nodeInfo.Emits[0];
            Assert.Equal("TestEvent", emit.Name.Code);
            Assert.Equal("测试事件", emit.Name.Display);
            Assert.NotNull(emit.EventInfo);
            Assert.Equal("TestEvent", emit.EventInfo.Name);
        }

        [Fact]
        public void CreateNodeInfo_WithNodeWithoutLabel_ShouldUseTypeNameAsLabel()
        {
            // Arrange
            var nodeType = typeof(NodeWithoutLabel);

            // Act
            var nodeInfo = NodeFactory.CreateNodeInfo(nodeType);

            // Assert
            Assert.Equal("NodeWithoutLabel", nodeInfo.Name.Code);
            Assert.Equal("", nodeInfo.Name.Display); // FlowNodeAttribute存在但Label为空字符串
        }

        [Fact]
        public void CreateNodeInfo_WithComplexNode_ShouldHaveAllFields()
        {
            // Arrange
            var nodeType = typeof(ComplexTestNode);

            // Act
            var nodeInfo = NodeFactory.CreateNodeInfo(nodeType);

            // Assert
            Assert.Equal(3, nodeInfo.InputFields.Count);
            Assert.Equal(2, nodeInfo.OutputFields.Count);
            Assert.Equal(2, nodeInfo.Signals.Count);
            Assert.Equal(2, nodeInfo.Emits.Count);
        }

        [Fact]
        public void CreateNodeInfo_WithComplexNode_ShouldHaveCorrectRequiredInput()
        {
            // Arrange
            var nodeType = typeof(ComplexTestNode);

            // Act
            var nodeInfo = NodeFactory.CreateNodeInfo(nodeType);

            // Assert
            var requiredInput = nodeInfo.InputFields.FirstOrDefault(f => f.Name.Code == "RequiredInput");
            Assert.NotNull(requiredInput);
            Assert.True(requiredInput.Required);
            Assert.Equal("必填输入", requiredInput.Name.Display);
        }

        [Fact]
        public void CreateNodeInfo_WithComplexNode_ShouldHaveCorrectOptionalInput()
        {
            // Arrange
            var nodeType = typeof(ComplexTestNode);

            // Act
            var nodeInfo = NodeFactory.CreateNodeInfo(nodeType);

            // Assert
            var optionalInput = nodeInfo.InputFields.FirstOrDefault(f => f.Name.Code == "OptionalInput");
            Assert.NotNull(optionalInput);
            Assert.False(optionalInput.Required);
            Assert.Equal("可选输入", optionalInput.Name.Display);
        }

        [Fact]
        public void CreateNodeInfo_WithComplexNode_ShouldHaveCorrectInputTypes()
        {
            // Arrange
            var nodeType = typeof(ComplexTestNode);

            // Act
            var nodeInfo = NodeFactory.CreateNodeInfo(nodeType);

            // Assert
            var stringInput = nodeInfo.InputFields.FirstOrDefault(f => f.Name.Code == "RequiredInput");
            var intInput = nodeInfo.InputFields.FirstOrDefault(f => f.Name.Code == "OptionalInput");
            var decimalInput = nodeInfo.InputFields.FirstOrDefault(f => f.Name.Code == "DecimalInput");

            Assert.Equal("String", stringInput.Type);
            Assert.Equal("Int32", intInput.Type);
            Assert.Equal("Decimal", decimalInput.Type);
        }

        [Fact]
        public void CreateNodeInfo_WithComplexNode_ShouldHaveCorrectOutputTypes()
        {
            // Arrange
            var nodeType = typeof(ComplexTestNode);

            // Act
            var nodeInfo = NodeFactory.CreateNodeInfo(nodeType);

            // Assert
            var stringOutput = nodeInfo.OutputFields.FirstOrDefault(f => f.Name.Code == "StringOutput");
            var numberOutput = nodeInfo.OutputFields.FirstOrDefault(f => f.Name.Code == "NumberOutput");

            Assert.Equal("String", stringOutput.Type);
            Assert.Equal("Int32", numberOutput.Type);
        }

        [Fact]
        public void CreateNodeInfo_WithComplexNode_ShouldHaveCorrectSignals()
        {
            // Arrange
            var nodeType = typeof(ComplexTestNode);

            // Act
            var nodeInfo = NodeFactory.CreateNodeInfo(nodeType);

            // Assert
            var signalWithoutParams = nodeInfo.Signals.FirstOrDefault(s => s.Name.Code == "SignalWithoutParams");
            var signalWithParams = nodeInfo.Signals.FirstOrDefault(s => s.Name.Code == "SignalWithParams");

            Assert.NotNull(signalWithoutParams);
            Assert.NotNull(signalWithParams);
            Assert.Equal("无参数信号", signalWithoutParams.Name.Display);
            Assert.Equal("有参数信号", signalWithParams.Name.Display);
        }

        [Fact]
        public void CreateNodeInfo_WithComplexNode_ShouldHaveCorrectEmits()
        {
            // Arrange
            var nodeType = typeof(ComplexTestNode);

            // Act
            var nodeInfo = NodeFactory.CreateNodeInfo(nodeType);

            // Assert
            var simpleEvent = nodeInfo.Emits.FirstOrDefault(e => e.Name.Code == "SimpleEvent");
            var complexEvent = nodeInfo.Emits.FirstOrDefault(e => e.Name.Code == "ComplexEvent");

            Assert.NotNull(simpleEvent);
            Assert.NotNull(complexEvent);
            Assert.Equal("简单事件", simpleEvent.Name.Display);
            Assert.Equal("带参数事件", complexEvent.Name.Display);
        }

        [Fact]
        public void CreateNodeInfo_WithNodeWithoutAttributes_ShouldCreateEmptyCollections()
        {
            // Arrange
            var nodeType = typeof(NodeWithoutAttributes);

            // Act
            var nodeInfo = NodeFactory.CreateNodeInfo(nodeType);

            // Assert
            Assert.Equal("NodeWithoutAttributes", nodeInfo.Name.Code);
            Assert.Equal("NodeWithoutAttributes", nodeInfo.Name.Display);
            Assert.Empty(nodeInfo.InputFields);
            Assert.Empty(nodeInfo.OutputFields);
            Assert.Empty(nodeInfo.Signals);
            Assert.Empty(nodeInfo.Emits);
        }

        [Fact]
        public void CreateNodeInfo_WithNullType_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => NodeFactory.CreateNodeInfo(null));
        }



        [Fact]
        public void CreateNodeInfo_ShouldPreservePropertyInfoReferences()
        {
            // Arrange
            var nodeType = typeof(SimpleTestNode);

            // Act
            var nodeInfo = NodeFactory.CreateNodeInfo(nodeType);

            // Assert
            var inputField = nodeInfo.InputFields[0];
            var outputField = nodeInfo.OutputFields[0];

            Assert.Equal(nodeType.GetProperty("Name"), inputField.PropertyInfo);
            Assert.Equal(nodeType.GetProperty("Result"), outputField.PropertyInfo);
        }

        [Fact]
        public void CreateNodeInfo_ShouldPreserveMethodInfoReferences()
        {
            // Arrange
            var nodeType = typeof(SimpleTestNode);

            // Act
            var nodeInfo = NodeFactory.CreateNodeInfo(nodeType);

            // Assert
            var signal = nodeInfo.Signals[0];
            Assert.Equal(nodeType.GetMethod("TestSignal"), signal.MethodInfo);
        }

        [Fact]
        public void CreateNodeInfo_ShouldPreserveEventInfoReferences()
        {
            // Arrange
            var nodeType = typeof(SimpleTestNode);

            // Act
            var nodeInfo = NodeFactory.CreateNodeInfo(nodeType);

            // Assert
            var emit = nodeInfo.Emits[0];
            Assert.Equal(nodeType.GetEvent("TestEvent"), emit.EventInfo);
        }

        [Fact]
        public void CreateNodeInfo_WithFlowExpression_ShouldCreateCorrectExpressionMeta()
        {
            // Arrange
            var nodeType = typeof(MathExpression);

            // Act
            var nodeInfo = NodeFactory.CreateNodeInfo(nodeType);

            // Assert
            Assert.NotNull(nodeInfo);
            Assert.Equal("MathExpression", nodeInfo.Name.Code);
            Assert.Equal("数学表达式", nodeInfo.Name.Display);
            Assert.Equal(nodeType, nodeInfo.NodeImplType);
            Assert.Equal(NodeKind.Expression, nodeInfo.Kind);
            Assert.NotNull(nodeInfo.InputFields);
            Assert.NotNull(nodeInfo.OutputFields);
            Assert.NotNull(nodeInfo.Signals);
            Assert.NotNull(nodeInfo.Emits);
        }

        [Fact]
        public void CreateNodeInfo_WithFlowExpression_ShouldHaveCorrectInputFields()
        {
            // Arrange
            var nodeType = typeof(MathExpression);

            // Act
            var nodeInfo = NodeFactory.CreateNodeInfo(nodeType);

            // Assert
            Assert.Equal(2, nodeInfo.InputFields.Count);
            var leftInput = nodeInfo.InputFields.FirstOrDefault(f => f.Name.Code == "Left");
            var rightInput = nodeInfo.InputFields.FirstOrDefault(f => f.Name.Code == "Right");

            Assert.NotNull(leftInput);
            Assert.NotNull(rightInput);
            Assert.Equal("左操作数", leftInput.Name.Display);
            Assert.Equal("右操作数", rightInput.Name.Display);
            Assert.Equal("Int32", leftInput.Type);
            Assert.Equal("Int32", rightInput.Type);
        }

        [Fact]
        public void CreateNodeInfo_WithFlowExpression_ShouldHaveCorrectOutputField()
        {
            // Arrange
            var nodeType = typeof(MathExpression);

            // Act
            var nodeInfo = NodeFactory.CreateNodeInfo(nodeType);

            // Assert
            Assert.Single(nodeInfo.OutputFields);
            var outputField = nodeInfo.OutputFields.FirstOrDefault(f => f.Name.Code == "Result");
            Assert.NotNull(outputField);
            Assert.Equal("计算结果", outputField.Name.Display);
            Assert.Equal("Int32", outputField.Type);
        }

        [Fact]
        public void CreateNodeInfo_WithFlowExpression_ShouldHaveCorrectSignal()
        {
            // Arrange
            var nodeType = typeof(MathExpression);

            // Act
            var nodeInfo = NodeFactory.CreateNodeInfo(nodeType);

            // Assert
            Assert.Single(nodeInfo.Signals);
            var signal = nodeInfo.Signals.FirstOrDefault(s => s.Name.Code == "Evaluate");
            Assert.NotNull(signal);
            Assert.Equal("执行", signal.Name.Display);
        }

        [Fact]
        public void CreateNodeInfo_WithFlowExpressionWithoutAttribute_ShouldUseTypeNameAsLabel()
        {
            // Arrange
            var nodeType = typeof(StringExpression);

            // Act
            var nodeInfo = NodeFactory.CreateNodeInfo(nodeType);

            // Assert
            Assert.Equal("StringExpression", nodeInfo.Name.Code);
            Assert.Equal("StringExpression", nodeInfo.Name.Display);
            Assert.Equal(NodeKind.Expression, nodeInfo.Kind);
        }

        [Fact]
        public void CreateNodeInfo_WithFlowExpression_ShouldHaveCorrectNodeKind()
        {
            // Arrange
            var nodeType = typeof(MathExpression);

            // Act
            var nodeInfo = NodeFactory.CreateNodeInfo(nodeType);

            // Assert
            Assert.Equal(NodeKind.Expression, nodeInfo.Kind);
        }

        [Fact]
        public void CreateNodeInfo_WithNormalNode_ShouldHaveCorrectNodeKind()
        {
            // Arrange
            var nodeType = typeof(SimpleTestNode);

            // Act
            var nodeInfo = NodeFactory.CreateNodeInfo(nodeType);

            // Assert
            Assert.Equal(NodeKind.Node, nodeInfo.Kind);
        }
    }
}
