using Flow.Core;
using Flow.Core.NodeMeta;
using Flow.Core.Metas;
using Flow.Common;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace Test.Flow.Graph
{
    public class FlowGraphTests
    {
        [Fact]
        public void Create_WithValidGraph_ShouldSucceed()
        {
            // Arrange
            var nodes = new List<FlowNodeInfo>
            {
                CreateStartNode(1),
                CreateSimpleNode(2)
            };
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Nodes.Count);
            Assert.Equal(0, result.Data.Connections.Count);
        }

        [Fact]
        public void Create_WithEmptyNodes_ShouldFail()
        {
            // Arrange
            var nodes = new List<FlowNodeInfo>();
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("图中没有节点", result.Message);
        }

        [Fact]
        public void Create_WithNullNodes_ShouldFail()
        {
            // Arrange
            List<FlowNodeInfo> nodes = null;
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("图中没有节点", result.Message);
        }

        [Fact]
        public void Create_WithNullNode_ShouldFail()
        {
            // Arrange
            var nodes = new List<FlowNodeInfo>
            {
                CreateStartNode(1),
                null
            };
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("图中存在空节点", result.Message);
        }

        [Fact]
        public void Create_WithoutStartNode_ShouldFail()
        {
            // Arrange
            var nodes = new List<FlowNodeInfo>
            {
                CreateSimpleNode(1),
                CreateSimpleNode(2)
            };
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("图中必须包含至少一个开始节点", result.Message);
        }

        [Fact]
        public void Create_WithMultipleStartNodes_ShouldFail()
        {
            // Arrange
            var nodes = new List<FlowNodeInfo>
            {
                CreateStartNode(1),
                CreateStartNode(2)
            };
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("图中只能有一个开始节点", result.Message);
        }

        [Fact]
        public void Create_WithDuplicateNodeIds_ShouldFail()
        {
            // Arrange
            var nodes = new List<FlowNodeInfo>
            {
                CreateStartNode(1),
                CreateSimpleNode(1) // 重复的ID
            };
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("节点ID重复: 1", result.Message);
        }

        [Fact]
        public void Create_WithNodeWithoutImplType_ShouldFail()
        {
            // Arrange
            var node = CreateStartNode(1);
            node.NodeImplType = null;
            var nodes = new List<FlowNodeInfo> { node };
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("节点 1 缺少实现类型", result.Message);
        }

        [Fact]
        public void Create_WithNodeWithoutLabel_ShouldFail()
        {
            // Arrange
            var node = CreateStartNode(1);
            node.Label = null;
            var nodes = new List<FlowNodeInfo> { node };
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("节点 1 缺少标签", result.Message);
        }

        [Fact]
        public void Create_WithDuplicateInputFieldCodes_ShouldFail()
        {
            // Arrange
            var node = CreateStartNode(1);
            node.InputFields = new List<InputField>
            {
                new InputField { Label = new Label { Code = "input1" } },
                new InputField { Label = new Label { Code = "input1" } } // 重复的代码
            };
            var nodes = new List<FlowNodeInfo> { node };
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("节点 1 输入字段代码重复: input1", result.Message);
        }

        [Fact]
        public void Create_WithDuplicateOutputFieldCodes_ShouldFail()
        {
            // Arrange
            var node = CreateStartNode(1);
            node.OutputFields = new List<OutputField>
            {
                new OutputField { Label = new Label { Code = "output1" } },
                new OutputField { Label = new Label { Code = "output1" } } // 重复的代码
            };
            var nodes = new List<FlowNodeInfo> { node };
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("节点 1 输出字段代码重复: output1", result.Message);
        }

        [Fact]
        public void Create_WithDuplicateSignalCodes_ShouldFail()
        {
            // Arrange
            var node = CreateStartNode(1);
            node.Signals = new List<SignalInfo>
            {
                new SignalInfo { Label = new Label { Code = "signal1" } },
                new SignalInfo { Label = new Label { Code = "signal1" } } // 重复的代码
            };
            var nodes = new List<FlowNodeInfo> { node };
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("节点 1 信号代码重复: signal1", result.Message);
        }

        [Fact]
        public void Create_WithDuplicateEmitCodes_ShouldFail()
        {
            // Arrange
            var node = CreateStartNode(1);
            node.Emits = new List<EmitInfo>
            {
                new EmitInfo { Label = new Label { Code = "emit1" } },
                new EmitInfo { Label = new Label { Code = "emit1" } } // 重复的代码
            };
            var nodes = new List<FlowNodeInfo> { node };
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("节点 1 事件代码重复: emit1", result.Message);
        }

        [Fact]
        public void Create_WithNullConnection_ShouldFail()
        {
            // Arrange
            var nodes = new List<FlowNodeInfo> { CreateStartNode(1) };
            var connections = new List<FlowConnection> { null };

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("图中存在空连接", result.Message);
        }

        [Fact]
        public void Create_WithDuplicateConnectionIds_ShouldFail()
        {
            // Arrange
            var node1 = CreateSimpleNode(1);
            node1.OutputFields = new List<OutputField> { new OutputField { Label = new Label { Code = "out1" } } };
            var node2 = CreateSimpleNode(2);
            node2.InputFields = new List<InputField> { new InputField { Label = new Label { Code = "in1" } } };
            var nodes = new List<FlowNodeInfo> { CreateStartNode(3), node1, node2 };
            var connections = new List<FlowConnection>
            {
                new FlowConnection { Id = 1, SourceNodeId = 1, TargetNodeId = 2, SourceEndpointCode = "out1", TargetEndpointCode = "in1", Type = ConnectionType.OutputToInput },
                new FlowConnection { Id = 1, SourceNodeId = 1, TargetNodeId = 2, SourceEndpointCode = "out1", TargetEndpointCode = "in1", Type = ConnectionType.OutputToInput }
            };

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("连接ID重复: 1", result.Message);
        }

        [Fact]
        public void Create_WithNonExistentSourceNode_ShouldFail()
        {
            // Arrange
            var nodes = new List<FlowNodeInfo> { CreateStartNode(1) };
            var connections = new List<FlowConnection>
            {
                new FlowConnection { SourceNodeId = 999, TargetNodeId = 1 }
            };

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("连接", result.Message);
            Assert.Contains("的源节点 999 不存在", result.Message);
        }

        [Fact]
        public void Create_WithNonExistentTargetNode_ShouldFail()
        {
            // Arrange
            var nodes = new List<FlowNodeInfo> { CreateStartNode(1) };
            var connections = new List<FlowConnection>
            {
                new FlowConnection { SourceNodeId = 1, TargetNodeId = 999 }
            };

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("连接", result.Message);
            Assert.Contains("的目标节点 999 不存在", result.Message);
        }

        [Fact]
        public void Create_WithSelfConnection_ShouldFail()
        {
            // Arrange
            var nodes = new List<FlowNodeInfo> { CreateStartNode(1) };
            var connections = new List<FlowConnection>
            {
                new FlowConnection { SourceNodeId = 1, TargetNodeId = 1 }
            };

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("连接", result.Message);
            Assert.Contains("不能连接到自身节点", result.Message);
        }

        [Fact]
        public void Create_WithValidOutputToInputConnection_ShouldSucceed()
        {
            // Arrange
            var sourceNode = CreateSimpleNode(1);
            sourceNode.OutputFields = new List<OutputField>
            {
                new OutputField { Label = new Label { Code = "output1" } }
            };

            var targetNode = CreateSimpleNode(2);
            targetNode.InputFields = new List<InputField>
            {
                new InputField { Label = new Label { Code = "input1" } }
            };

            var nodes = new List<FlowNodeInfo> { CreateStartNode(3), sourceNode, targetNode };
            var connections = new List<FlowConnection>
            {
                new FlowConnection
                {
                    Type = ConnectionType.OutputToInput,
                    SourceNodeId = 1,
                    TargetNodeId = 2,
                    SourceEndpointCode = "output1",
                    TargetEndpointCode = "input1"
                }
            };

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void Create_WithInvalidOutputToInputConnection_ShouldFail()
        {
            // Arrange
            var sourceNode = CreateSimpleNode(1);
            sourceNode.OutputFields = new List<OutputField>
            {
                new OutputField { Label = new Label { Code = "output1" } }
            };

            var targetNode = CreateSimpleNode(2);
            targetNode.InputFields = new List<InputField>
            {
                new InputField { Label = new Label { Code = "input1" } }
            };

            var nodes = new List<FlowNodeInfo> { CreateStartNode(3), sourceNode, targetNode };
            var connections = new List<FlowConnection>
            {
                new FlowConnection
                {
                    Type = ConnectionType.OutputToInput,
                    SourceNodeId = 1,
                    TargetNodeId = 2,
                    SourceEndpointCode = "nonexistent",
                    TargetEndpointCode = "input1"
                }
            };

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("不存在输出字段 nonexistent", result.Message);
        }

        [Fact]
        public void Create_WithValidEventToSignalConnection_ShouldSucceed()
        {
            // Arrange
            var sourceNode = CreateSimpleNode(1);
            sourceNode.Emits = new List<EmitInfo>
            {
                new EmitInfo { Label = new Label { Code = "event1" } }
            };

            var targetNode = CreateSimpleNode(2);
            targetNode.Signals = new List<SignalInfo>
            {
                new SignalInfo { Label = new Label { Code = "signal1" } }
            };

            var nodes = new List<FlowNodeInfo> { CreateStartNode(3), sourceNode, targetNode };
            var connections = new List<FlowConnection>
            {
                new FlowConnection
                {
                    Type = ConnectionType.EventToSignal,
                    SourceNodeId = 1,
                    TargetNodeId = 2,
                    SourceEndpointCode = "event1",
                    TargetEndpointCode = "signal1"
                }
            };

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void Create_WithInvalidEventToSignalConnection_ShouldFail()
        {
            // Arrange
            var sourceNode = CreateSimpleNode(1);
            sourceNode.Emits = new List<EmitInfo>
            {
                new EmitInfo { Label = new Label { Code = "event1" } }
            };

            var targetNode = CreateSimpleNode(2);
            targetNode.Signals = new List<SignalInfo>
            {
                new SignalInfo { Label = new Label { Code = "signal1" } }
            };

            var nodes = new List<FlowNodeInfo> { CreateStartNode(3), sourceNode, targetNode };
            var connections = new List<FlowConnection>
            {
                new FlowConnection
                {
                    Type = ConnectionType.EventToSignal,
                    SourceNodeId = 1,
                    TargetNodeId = 2,
                    SourceEndpointCode = "nonexistent",
                    TargetEndpointCode = "signal1"
                }
            };

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("不存在事件 nonexistent", result.Message);
        }

        [Fact]
        public void Create_WithMultipleEventConnections_ShouldFail()
        {
            // Arrange
            var sourceNode = CreateSimpleNode(1);
            sourceNode.Emits = new List<EmitInfo>
            {
                new EmitInfo { Label = new Label { Code = "event1" } }
            };

            var targetNode1 = CreateSimpleNode(2);
            targetNode1.Signals = new List<SignalInfo>
            {
                new SignalInfo { Label = new Label { Code = "signal1" } }
            };

            var targetNode2 = CreateSimpleNode(3);
            targetNode2.Signals = new List<SignalInfo>
            {
                new SignalInfo { Label = new Label { Code = "signal2" } }
            };

            var nodes = new List<FlowNodeInfo> { CreateStartNode(4), sourceNode, targetNode1, targetNode2 };
            var connections = new List<FlowConnection>
            {
                new FlowConnection
                {
                    Type = ConnectionType.EventToSignal,
                    SourceNodeId = 1,
                    TargetNodeId = 2,
                    SourceEndpointCode = "event1",
                    TargetEndpointCode = "signal1"
                },
                new FlowConnection
                {
                    Type = ConnectionType.EventToSignal,
                    SourceNodeId = 1,
                    TargetNodeId = 3,
                    SourceEndpointCode = "event1",
                    TargetEndpointCode = "signal2"
                }
            };

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("事件 event1 连接了多个信号", result.Message);
        }

        [Fact]
        public void Create_WithRequiredInputFieldWithoutConnection_ShouldFail()
        {
            // Arrange
            var node = CreateSimpleNode(1);
            node.InputFields = new List<InputField>
            {
                new InputField 
                { 
                    Label = new Label { Code = "requiredInput" },
                    Required = true
                }
            };

            var nodes = new List<FlowNodeInfo> { CreateStartNode(2), node };
            var connections = new List<FlowConnection>();

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("必填输入字段 requiredInput 没有连接", result.Message);
        }

        [Fact]
        public void Create_WithRequiredInputFieldWithConnection_ShouldSucceed()
        {
            // Arrange
            var sourceNode = CreateSimpleNode(1);
            sourceNode.OutputFields = new List<OutputField>
            {
                new OutputField { Label = new Label { Code = "output1" } }
            };

            var targetNode = CreateSimpleNode(2);
            targetNode.InputFields = new List<InputField>
            {
                new InputField 
                { 
                    Label = new Label { Code = "requiredInput" },
                    Required = true
                }
            };

            var nodes = new List<FlowNodeInfo> { CreateStartNode(3), sourceNode, targetNode };
            var connections = new List<FlowConnection>
            {
                new FlowConnection
                {
                    Type = ConnectionType.OutputToInput,
                    SourceNodeId = 1,
                    TargetNodeId = 2,
                    SourceEndpointCode = "output1",
                    TargetEndpointCode = "requiredInput"
                }
            };

            // Act
            var result = FlowGraph.Create(nodes, connections);

            // Assert
            Assert.True(result.IsSuccess);
        }

        private FlowNodeInfo CreateStartNode(long id)
        {
            return new FlowNodeInfo
            {
                Id = id,
                Label = new Label { Code = "StartNode", DisplayName = "开始节点" },
                NodeImplType = typeof(object),
                Kind = NodeKind.StartNode
            };
        }

        private FlowNodeInfo CreateSimpleNode(long id)
        {
            return new FlowNodeInfo
            {
                Id = id,
                Label = new Label { Code = "SimpleNode", DisplayName = "简单节点" },
                NodeImplType = typeof(object),
                Kind = NodeKind.Node
            };
        }
    }
} 