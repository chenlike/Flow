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
using JstFlow.External;

namespace Test.JstFlow.Graph
{
    public class GraphValidateTests
    {
        #region 测试节点类定义

        [FlowNode("测试节点1")]
        public class TestNode1
        {
            [Input("输入1")]
            public string Input1 { get; set; }

            [Input("输入2", Required = true)]
            public string Input2 { get; set; }

            [Output("输出1")]
            public string Output1 { get; set; }

            [Output("输出2")]
            public string Output2 { get; set; }

            [Signal("信号1")]
            public void Signal1() { }

            [Emit("事件1")]
            public event Action Event1;
        }

        [FlowNode("测试节点2")]
        public class TestNode2
        {
            [Input("输入A")]
            public string InputA { get; set; }

            [Output("输出A")]
            public string OutputA { get; set; }

            [Signal("信号A")]
            public void SignalA() { }

            [Emit("事件A")]
            public event Action EventA;
        }

        [FlowNode("无效节点")]
        public class InvalidNode
        {
            // 节点没有Label的字段
            public string Input1 { get; set; }
        }

        #endregion

        #region 辅助方法

        private FlowNode CreateTestNode1()
        {
            return new FlowNode
            {
                Id = 1,
                Label = new Label("TestNode1", "测试节点1"),
                NodeImplType = typeof(TestNode1),
                Kind = NodeKind.Node,
                InputFields = new List<InputField>
                {
                    new InputField { Label = new Label("Input1", "输入1"), Required = false },
                    new InputField { Label = new Label("Input2", "输入2"), Required = false }
                },
                OutputFields = new List<OutputField>
                {
                    new OutputField { Label = new Label("Output1", "输出1") },
                    new OutputField { Label = new Label("Output2", "输出2") }
                },
                Signals = new List<SignalInfo>
                {
                    new SignalInfo { Label = new Label("Signal1", "信号1") }
                },
                Emits = new List<EmitInfo>
                {
                    new EmitInfo { Label = new Label("Event1", "事件1") }
                }
            };
        }

        private FlowNode CreateTestNode2()
        {
            return new FlowNode
            {
                Id = 2,
                Label = new Label("TestNode2", "测试节点2"),
                NodeImplType = typeof(TestNode2),
                Kind = NodeKind.Node,
                InputFields = new List<InputField>
                {
                    new InputField { Label = new Label("InputA", "输入A"), Required = false }
                },
                OutputFields = new List<OutputField>
                {
                    new OutputField { Label = new Label("OutputA", "输出A") }
                },
                Signals = new List<SignalInfo>
                {
                    new SignalInfo { Label = new Label("SignalA", "信号A") }
                },
                Emits = new List<EmitInfo>
                {
                    new EmitInfo { Label = new Label("EventA", "事件A") }
                }
            };
        }

        private FlowConnection CreateOutputToInputConnection(long sourceNodeId, string sourceEndpoint, long targetNodeId, string targetEndpoint)
        {
            return new FlowConnection
            {
                Type = ConnectionType.OutputToInput,
                SourceNodeId = sourceNodeId,
                SourceEndpointCode = sourceEndpoint,
                TargetNodeId = targetNodeId,
                TargetEndpointCode = targetEndpoint
            };
        }

        private FlowConnection CreateEventToSignalConnection(long sourceNodeId, string sourceEndpoint, long targetNodeId, string targetEndpoint)
        {
            return new FlowConnection
            {
                Type = ConnectionType.EventToSignal,
                SourceNodeId = sourceNodeId,
                SourceEndpointCode = sourceEndpoint,
                TargetNodeId = targetNodeId,
                TargetEndpointCode = targetEndpoint
            };
        }

        private List<FlowNode> AddStartNodeToNodes(List<FlowNode> nodes)
        {
            var result = new List<FlowNode> { CreateStartNode() };
            result.AddRange(nodes);
            return result;
        }

        private FlowNode CreateStartNode()
        {
            return new FlowNode
            {
                Id = 100,
                Label = new Label("StartNode", "开始节点"),
                NodeImplType = typeof(StartNode),
                Kind = NodeKind.StartNode,
                InputFields = new List<InputField>(),
                OutputFields = new List<OutputField>(),
                Signals = new List<SignalInfo>
                {
                    new SignalInfo { Label = new Label("StartLoop", "开始循环") }
                },
                Emits = new List<EmitInfo>
                {
                    new EmitInfo { Label = new Label("Start", "开始执行") }
                }
            };
        }

        #endregion

        #region StartNode验证测试

        [Fact]
        public void ValidateGraph_NoStartNode_ShouldReturnFailure()
        {
            // Arrange
            var nodes = new List<FlowNode> { CreateTestNode1() };
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("图中必须包含至少一个开始节点", result.Message);
        }

        [Fact]
        public void ValidateGraph_MultipleStartNodes_ShouldReturnFailure()
        {
            // Arrange
            var startNode1 = CreateStartNode();
            var startNode2 = CreateStartNode();
            startNode2.Id = 101;
            var nodes = new List<FlowNode> { startNode1, startNode2 };
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("图中只能有一个开始节点", result.Message);
        }

        [Fact]
        public void ValidateGraph_WithStartNode_ShouldReturnSuccess()
        {
            // Arrange
            var startNode = CreateStartNode();
            var nodes = new List<FlowNode> { startNode };
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("图验证通过", result.Message);
        }

        [Fact]
        public void ValidateGraph_StartNodeWithOtherNodes_ShouldReturnSuccess()
        {
            // Arrange
            var startNode = CreateStartNode();
            var testNode = CreateTestNode1();
            var nodes = new List<FlowNode> { startNode, testNode };
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("图验证通过", result.Message);
        }

        #endregion

        #region 基本验证测试

        [Fact]
        public void ValidateGraph_EmptyNodes_ShouldReturnFailure()
        {
            // Arrange
            var nodes = new List<FlowNode>();
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("图中没有节点", result.Message);
        }

        [Fact]
        public void ValidateGraph_NullNodes_ShouldReturnFailure()
        {
            // Arrange
            List<FlowNode> nodes = null;
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("图中没有节点", result.Message);
        }

        [Fact]
        public void ValidateGraph_ValidSingleNode_ShouldReturnSuccess()
        {
            // Arrange
            var startNode = CreateStartNode();
            var testNode = CreateTestNode1();
            var nodes = new List<FlowNode> { startNode, testNode };
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("图验证通过", result.Message);
        }

        #endregion

        #region 节点验证测试

        [Fact]
        public void ValidateGraph_NullNode_ShouldReturnFailure()
        {
            // Arrange
            var startNode = CreateStartNode();
            var nodes = new List<FlowNode> { startNode, null };
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("图中存在空节点", result.Message);
        }

        [Fact]
        public void ValidateGraph_DuplicateNodeId_ShouldReturnFailure()
        {
            // Arrange
            var startNode = CreateStartNode();
            var node1 = CreateTestNode1();
            var node2 = CreateTestNode2();
            node2.Id = node1.Id; // 重复ID
            var nodes = new List<FlowNode> { startNode, node1, node2 };
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("节点ID重复", result.Message);
        }

        [Fact]
        public void ValidateGraph_NodeWithoutImplType_ShouldReturnFailure()
        {
            // Arrange
            var startNode = CreateStartNode();
            var node = CreateTestNode1();
            node.NodeImplType = null;
            var nodes = new List<FlowNode> { startNode, node };
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("缺少实现类型", result.Message);
        }

        [Fact]
        public void ValidateGraph_NodeWithoutLabel_ShouldReturnFailure()
        {
            // Arrange
            var startNode = CreateStartNode();
            var node = CreateTestNode1();
            node.Label = null;
            var nodes = new List<FlowNode> { startNode, node };
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("缺少标签", result.Message);
        }

        #endregion

        #region 输入字段验证测试

        [Fact]
        public void ValidateGraph_NullInputField_ShouldReturnFailure()
        {
            // Arrange
            var node = CreateTestNode1();
            node.InputFields.Add(null);
            var nodes = AddStartNodeToNodes(new List<FlowNode> { node });
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("存在空输入字段", result.Message);
        }

        [Fact]
        public void ValidateGraph_InputFieldWithoutCode_ShouldReturnFailure()
        {
            // Arrange
            var startNode = CreateStartNode();
            var node = CreateTestNode1();
            node.InputFields.Add(new InputField { Label = new Label("", "测试") });
            var nodes = new List<FlowNode> { startNode, node };
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("输入字段缺少代码", result.Message);
        }

        [Fact]
        public void ValidateGraph_DuplicateInputFieldCode_ShouldReturnFailure()
        {
            // Arrange
            var node = CreateTestNode1();
            node.InputFields.Add(new InputField { Label = new Label("Input1", "重复输入") });
            var nodes = AddStartNodeToNodes(new List<FlowNode> { node });
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("输入字段代码重复", result.Message);
        }

        #endregion

        #region 输出字段验证测试

        [Fact]
        public void ValidateGraph_NullOutputField_ShouldReturnFailure()
        {
            // Arrange
            var node = CreateTestNode1();
            node.OutputFields.Add(null);
            var nodes = AddStartNodeToNodes(new List<FlowNode> { node });
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("存在空输出字段", result.Message);
        }

        [Fact]
        public void ValidateGraph_OutputFieldWithoutCode_ShouldReturnFailure()
        {
            // Arrange
            var node = CreateTestNode1();
            node.OutputFields.Add(new OutputField { Label = new Label("", "测试") });
            var nodes = AddStartNodeToNodes(new List<FlowNode> { node });
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("输出字段缺少代码", result.Message);
        }

        [Fact]
        public void ValidateGraph_DuplicateOutputFieldCode_ShouldReturnFailure()
        {
            // Arrange
            var node = CreateTestNode1();
            node.OutputFields.Add(new OutputField { Label = new Label("Output1", "重复输出") });
            var nodes = AddStartNodeToNodes(new List<FlowNode> { node });
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("输出字段代码重复", result.Message);
        }

        #endregion

        #region 信号验证测试

        [Fact]
        public void ValidateGraph_NullSignal_ShouldReturnFailure()
        {
            // Arrange
            var node = CreateTestNode1();
            node.Signals.Add(null);
            var nodes = AddStartNodeToNodes(new List<FlowNode> { node });
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("存在空信号", result.Message);
        }

        [Fact]
        public void ValidateGraph_SignalWithoutCode_ShouldReturnFailure()
        {
            // Arrange
            var node = CreateTestNode1();
            node.Signals.Add(new SignalInfo { Label = new Label("", "测试") });
            var nodes = AddStartNodeToNodes(new List<FlowNode> { node });
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("信号缺少代码", result.Message);
        }

        [Fact]
        public void ValidateGraph_DuplicateSignalCode_ShouldReturnFailure()
        {
            // Arrange
            var node = CreateTestNode1();
            node.Signals.Add(new SignalInfo { Label = new Label("Signal1", "重复信号") });
            var nodes = AddStartNodeToNodes(new List<FlowNode> { node });
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("信号代码重复", result.Message);
        }

        #endregion

        #region 事件验证测试

        [Fact]
        public void ValidateGraph_NullEmit_ShouldReturnFailure()
        {
            // Arrange
            var node = CreateTestNode1();
            node.Emits.Add(null);
            var nodes = AddStartNodeToNodes(new List<FlowNode> { node });
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("存在空事件", result.Message);
        }

        [Fact]
        public void ValidateGraph_EmitWithoutCode_ShouldReturnFailure()
        {
            // Arrange
            var node = CreateTestNode1();
            node.Emits.Add(new EmitInfo { Label = new Label("", "测试") });
            var nodes = AddStartNodeToNodes(new List<FlowNode> { node });
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("事件缺少代码", result.Message);
        }

        [Fact]
        public void ValidateGraph_DuplicateEmitCode_ShouldReturnFailure()
        {
            // Arrange
            var node = CreateTestNode1();
            node.Emits.Add(new EmitInfo { Label = new Label("Event1", "重复事件") });
            var nodes = AddStartNodeToNodes(new List<FlowNode> { node });
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("事件代码重复", result.Message);
        }

        #endregion

        #region 连接验证测试

        [Fact]
        public void ValidateGraph_NullConnection_ShouldReturnFailure()
        {
            // Arrange
            var nodes = AddStartNodeToNodes(new List<FlowNode> { CreateTestNode1() });
            var connections = new List<FlowConnection> { null };

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("图中存在空连接", result.Message);
        }

        [Fact]
        public void ValidateGraph_DuplicateConnectionId_ShouldReturnFailure()
        {
            // Arrange
            var node1 = CreateTestNode1();
            var node2 = CreateTestNode2();
            var nodes = AddStartNodeToNodes(new List<FlowNode> { node1, node2 });
            
            var connection1 = CreateOutputToInputConnection(node1.Id, "Output1", node2.Id, "InputA");
            var connection2 = CreateOutputToInputConnection(node1.Id, "Output2", node2.Id, "InputA");
            connection2.Id = connection1.Id; // 重复ID
            
            var connections = new List<FlowConnection> { connection1, connection2 };

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("连接ID重复", result.Message);
        }

        [Fact]
        public void ValidateGraph_ConnectionWithNonExistentSourceNode_ShouldReturnFailure()
        {
            // Arrange
            var node1 = CreateTestNode1();
            var nodes = AddStartNodeToNodes(new List<FlowNode> { node1 });
            var connections = new List<FlowConnection> 
            { 
                CreateOutputToInputConnection(999, "Output1", node1.Id, "Input1") 
            };

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("源节点 999 不存在", result.Message);
        }

        [Fact]
        public void ValidateGraph_ConnectionWithNonExistentTargetNode_ShouldReturnFailure()
        {
            // Arrange
            var node1 = CreateTestNode1();
            var nodes = AddStartNodeToNodes(new List<FlowNode> { node1 });
            var connections = new List<FlowConnection> 
            { 
                CreateOutputToInputConnection(node1.Id, "Output1", 999, "Input1") 
            };

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("目标节点 999 不存在", result.Message);
        }

        [Fact]
        public void ValidateGraph_ConnectionWithNonExistentOutputField_ShouldReturnFailure()
        {
            // Arrange
            var node1 = CreateTestNode1();
            var node2 = CreateTestNode2();
            var nodes = AddStartNodeToNodes(new List<FlowNode> { node1, node2 });
            var connections = new List<FlowConnection> 
            { 
                CreateOutputToInputConnection(node1.Id, "NonExistentOutput", node2.Id, "InputA") 
            };

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("不存在输出字段 NonExistentOutput", result.Message);
        }

        [Fact]
        public void ValidateGraph_ConnectionWithNonExistentInputField_ShouldReturnFailure()
        {
            // Arrange
            var node1 = CreateTestNode1();
            var node2 = CreateTestNode2();
            var nodes = AddStartNodeToNodes(new List<FlowNode> { node1, node2 });
            var connections = new List<FlowConnection> 
            { 
                CreateOutputToInputConnection(node1.Id, "Output1", node2.Id, "NonExistentInput") 
            };

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("不存在输入字段 NonExistentInput", result.Message);
        }

        [Fact]
        public void ValidateGraph_ConnectionWithNonExistentEvent_ShouldReturnFailure()
        {
            // Arrange
            var node1 = CreateTestNode1();
            var node2 = CreateTestNode2();
            var nodes = AddStartNodeToNodes(new List<FlowNode> { node1, node2 });
            var connections = new List<FlowConnection> 
            { 
                CreateEventToSignalConnection(node1.Id, "NonExistentEvent", node2.Id, "SignalA") 
            };

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("不存在事件 NonExistentEvent", result.Message);
        }

        [Fact]
        public void ValidateGraph_ConnectionWithNonExistentSignal_ShouldReturnFailure()
        {
            // Arrange
            var node1 = CreateTestNode1();
            var node2 = CreateTestNode2();
            var nodes = AddStartNodeToNodes(new List<FlowNode> { node1, node2 });
            var connections = new List<FlowConnection> 
            { 
                CreateEventToSignalConnection(node1.Id, "Event1", node2.Id, "NonExistentSignal") 
            };

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("不存在信号 NonExistentSignal", result.Message);
        }

        [Fact]
        public void ValidateGraph_SelfConnection_ShouldReturnFailure()
        {
            // Arrange
            var node1 = CreateTestNode1();
            var nodes = AddStartNodeToNodes(new List<FlowNode> { node1 });
            var connections = new List<FlowConnection> 
            { 
                CreateOutputToInputConnection(node1.Id, "Output1", node1.Id, "Input1") 
            };

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("不能连接到自身节点", result.Message);
        }

        [Fact]
        public void ValidateGraph_ValidOutputToInputConnection_ShouldReturnSuccess()
        {
            // Arrange
            var startNode = CreateStartNode();
            var node1 = CreateTestNode1();
            var node2 = CreateTestNode2();
            var nodes = new List<FlowNode> { startNode, node1, node2 };
            var connections = new List<FlowConnection> 
            { 
                CreateOutputToInputConnection(node1.Id, "Output1", node2.Id, "InputA") 
            };

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void ValidateGraph_ValidEventToSignalConnection_ShouldReturnSuccess()
        {
            // Arrange
            var startNode = CreateStartNode();
            var node1 = CreateTestNode1();
            var node2 = CreateTestNode2();
            var nodes = new List<FlowNode> { startNode, node1, node2 };
            var connections = new List<FlowConnection> 
            { 
                CreateEventToSignalConnection(node1.Id, "Event1", node2.Id, "SignalA") 
            };

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.True(result.IsSuccess);
        }

        #endregion

        #region 事件连接限制测试

        [Fact]
        public void ValidateGraph_EventConnectedToMultipleSignals_ShouldReturnFailure()
        {
            // Arrange
            var node1 = CreateTestNode1();
            var node2 = CreateTestNode2();
            var node3 = CreateTestNode1();
            node3.Id = 3;
            var nodes = AddStartNodeToNodes(new List<FlowNode> { node1, node2, node3 });
            
            var connections = new List<FlowConnection> 
            { 
                CreateEventToSignalConnection(node1.Id, "Event1", node2.Id, "SignalA"),
                CreateEventToSignalConnection(node1.Id, "Event1", node3.Id, "Signal1")
            };

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("连接了多个信号，一个事件只能连接一个信号", result.Message);
        }

        #endregion

        #region 必填字段验证测试

        [Fact]
        public void ValidateGraph_RequiredInputFieldWithoutConnection_ShouldReturnFailure()
        {
            // Arrange
            var startNode = CreateStartNode();
            var node = new FlowNode
            {
                Id = 1,
                Label = new Label("TestNode1", "测试节点1"),
                NodeImplType = typeof(TestNode1),
                Kind = NodeKind.Node,
                InputFields = new List<InputField>
                {
                    new InputField { Label = new Label("Input1", "输入1"), Required = false },
                    new InputField { Label = new Label("Input2", "输入2"), Required = true }
                },
                OutputFields = new List<OutputField>
                {
                    new OutputField { Label = new Label("Output1", "输出1") },
                    new OutputField { Label = new Label("Output2", "输出2") }
                },
                Signals = new List<SignalInfo>
                {
                    new SignalInfo { Label = new Label("Signal1", "信号1") }
                },
                Emits = new List<EmitInfo>
                {
                    new EmitInfo { Label = new Label("Event1", "事件1") }
                }
            };
            var nodes = new List<FlowNode> { startNode, node };
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("必填输入字段 Input2 没有连接", result.Message);
        }

        [Fact]
        public void ValidateGraph_RequiredInputFieldWithConnection_ShouldReturnSuccess()
        {
            // Arrange
            var startNode = CreateStartNode();
            var node1 = CreateTestNode1();
            var node2 = CreateTestNode2();
            var nodes = new List<FlowNode> { startNode, node1, node2 };
            var connections = new List<FlowConnection> 
            { 
                CreateOutputToInputConnection(node2.Id, "OutputA", node1.Id, "Input2") 
            };

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.True(result.IsSuccess);
        }

        #endregion

        #region 复合验证测试

        [Fact]
        public void ValidateGraph_ComplexValidGraph_ShouldReturnSuccess()
        {
            // Arrange
            var startNode = CreateStartNode();
            var node1 = CreateTestNode1();
            var node2 = CreateTestNode2();
            var node3 = CreateTestNode1();
            node3.Id = 3;
            node3.Label = new Label("TestNode3", "测试节点3");
            
            var nodes = new List<FlowNode> { startNode, node1, node2, node3 };
            var connections = new List<FlowConnection> 
            { 
                CreateOutputToInputConnection(node2.Id, "OutputA", node1.Id, "Input2"),
                CreateOutputToInputConnection(node1.Id, "Output1", node3.Id, "Input1"),
                CreateOutputToInputConnection(node1.Id, "Output2", node3.Id, "Input2"), // 为node3的必填字段添加连接
                CreateEventToSignalConnection(node1.Id, "Event1", node2.Id, "SignalA")
            };

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("图验证通过", result.Message);
        }

        #endregion
    }
}
