using JstFlow.Internal;
using JstFlow.Internal.NodeMeta;
using JstFlow.Internal.Metas;
using JstFlow.Common;
using JstFlow.External;
using System;
using System.Linq;
using Xunit;
using System.Collections.Generic;

namespace Test.JstFlow.Node
{
    public class FlowGraphTests
    {
        [Fact]
        public void TestFlowGraph_AddConnection_WithValidator()
        {
            // 创建图
            var graph = new FlowGraph();

            // 创建测试节点
            var node1 = CreateTestNode("Node1", Guid.NewGuid());
            var node2 = CreateTestNode("Node2", Guid.NewGuid());

            // 添加节点
            graph.AddNode(node1);
            graph.AddNode(node2);

            // 创建连接
            var connection = new FlowConnection
            {
                Type = ConnectionType.OutputToInput,
                SourceNodeId = node1.Id,
                TargetNodeId = node2.Id,
                SourceEndpointCode = "Output1",
                TargetEndpointCode = "Input1"
            };

            // 添加连接
            graph.AddConnection(connection);

            // 验证连接已添加
            Assert.Single(graph.Connections);
            Assert.Equal(connection.Id, graph.Connections.First().Id);
        }

        [Fact]
        public void TestFlowGraph_CircularDependency_Detection()
        {
            // 创建图
            var graph = new FlowGraph();

            // 创建三个节点
            var node1 = CreateTestNode("Node1", Guid.NewGuid());
            var node2 = CreateTestNode("Node2", Guid.NewGuid());
            var node3 = CreateTestNode("Node3", Guid.NewGuid());

            // 添加节点
            graph.AddNode(node1);
            graph.AddNode(node2);
            graph.AddNode(node3);

            // 创建循环连接：node1 -> node2 -> node3 -> node1
            var connection1 = new FlowConnection
            {
                Type = ConnectionType.OutputToInput,
                SourceNodeId = node1.Id,
                TargetNodeId = node2.Id,
                SourceEndpointCode = "Output1",
                TargetEndpointCode = "Input1"
            };

            var connection2 = new FlowConnection
            {
                Type = ConnectionType.OutputToInput,
                SourceNodeId = node2.Id,
                TargetNodeId = node3.Id,
                SourceEndpointCode = "Output1",
                TargetEndpointCode = "Input1"
            };

            var connection3 = new FlowConnection
            {
                Type = ConnectionType.OutputToInput,
                SourceNodeId = node3.Id,
                TargetNodeId = node1.Id,
                SourceEndpointCode = "Output1",
                TargetEndpointCode = "Input1"
            };

            // 添加前两个连接
            graph.AddConnection(connection1);
            graph.AddConnection(connection2);

            // 验证前两个连接不会形成循环
            Assert.False(graph.HasCircularDependency());

            // 验证添加第三个连接时会抛出异常（因为会形成循环）
            var exception = Assert.Throws<ArgumentException>(() =>
                graph.AddConnection(connection3));

            Assert.Contains("检测到循环依赖", exception.Message);
        }

        [Fact]
        public void TestFlowGraph_TopologicalSort()
        {
            // 创建图
            var graph = new FlowGraph();

            // 创建节点
            var node1 = CreateTestNode("Node1", Guid.NewGuid());
            var node2 = CreateTestNode("Node2", Guid.NewGuid());
            var node3 = CreateTestNode("Node3", Guid.NewGuid());

            // 添加节点
            graph.AddNode(node1);
            graph.AddNode(node2);
            graph.AddNode(node3);

            // 创建连接：node1 -> node2 -> node3
            var connection1 = new FlowConnection
            {
                Type = ConnectionType.OutputToInput,
                SourceNodeId = node1.Id,
                TargetNodeId = node2.Id,
                SourceEndpointCode = "Output1",
                TargetEndpointCode = "Input1"
            };

            var connection2 = new FlowConnection
            {
                Type = ConnectionType.OutputToInput,
                SourceNodeId = node2.Id,
                TargetNodeId = node3.Id,
                SourceEndpointCode = "Output1",
                TargetEndpointCode = "Input1"
            };

            // 添加连接
            graph.AddConnection(connection1);
            graph.AddConnection(connection2);

            // 获取拓扑排序
            var sortedNodes = graph.GetTopologicalSort();

            // 验证排序结果
            Assert.Equal(3, sortedNodes.Count);
            Assert.Equal(node1.Id, sortedNodes[0].Id);
            Assert.Equal(node2.Id, sortedNodes[1].Id);
            Assert.Equal(node3.Id, sortedNodes[2].Id);
        }






        private FlowNode CreateTestNode(string name, Guid id)
        {
            return new FlowNode
            {
                Id = id,
                Label = new Label(name, name),
                Kind = NodeKind.Node,
                InputFields = new List<InputField>
                {
                    new InputField
                    {
                        Label = new Label("Input1", "Input1"),
                        Type = "string",
                        Required = false
                    }
                },
                OutputFields = new List<OutputField>
                {
                    new OutputField
                    {
                        Label = new Label("Output1", "Output1"),
                        Type = "string"
                    }
                },
                Signals = new List<SignalInfo>(),
                Emits = new List<EmitInfo>()
            };
        }
    }
}