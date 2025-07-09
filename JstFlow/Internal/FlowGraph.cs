using JstFlow.Internal.NodeMeta;
using JstFlow.Internal.Metas;
using System;
using System.Collections.Generic;
using System.Text;
using JstFlow.Common;
using System.Linq;
using JstFlow.Internal.Utils;

namespace JstFlow.Internal
{
    /// <summary>
    /// 流程图，包含节点和连接
    /// </summary>
    public class FlowGraph
    {
        /// <summary>
        /// 图的唯一标识
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 节点
        /// </summary>
        public List<FlowNode> Nodes { get; set; } = new List<FlowNode>();

        /// <summary>
        /// 连接
        /// </summary>
        public List<FlowConnection> Connections { get; set; } = new List<FlowConnection>();

        /// <summary>
        /// 连接验证器
        /// </summary>
        private ConnectionValidator _validator;

        /// <summary>
        /// 构造函数
        /// </summary>
        public FlowGraph()
        {
            _validator = new ConnectionValidator(Nodes);
        }



        /// <summary>
        /// 添加节点
        /// </summary>
        public void AddNode(FlowNode node)
        {
            if (Nodes.Any(n => n.Id == node.Id))
            {
                throw new ArgumentException("节点已存在", nameof(node));
            }
            Nodes.Add(node);
            UpdateValidator();
        }

        /// <summary>
        /// 移除节点
        /// </summary>
        public void RemoveNode(Guid nodeId)
        {
            if (!Nodes.Any(n => n.Id == nodeId))
            {
                throw new ArgumentException("节点不存在", nameof(nodeId));
            }
            Nodes.Remove(Nodes.First(n => n.Id == nodeId));
            Connections.RemoveAll(c => c.SourceNodeId == nodeId || c.TargetNodeId == nodeId);
            UpdateValidator();
        }

        public FlowNode GetNode(Guid nodeId)
        {
            return Nodes.FirstOrDefault(n => n.Id == nodeId);
        }

        /// <summary>
        /// 添加连接
        /// </summary>
        public void AddConnection(FlowConnection connection)
        {
            // 使用验证器进行验证
            var validationResult = _validator.ValidateConnection(connection, Connections);
            
            if (!validationResult.IsValid)
            {
                throw new ArgumentException(validationResult.GetErrorMessage(), nameof(connection));
            }

            Connections.Add(connection);
        }

        /// <summary>
        /// 移除连接
        /// </summary>
        public void RemoveConnection(Guid connectionId)
        {
            if (!Connections.Any(c => c.Id == connectionId))
            {
                throw new ArgumentException("连接不存在", nameof(connectionId));
            }
            Connections.Remove(Connections.First(c => c.Id == connectionId));
        }

        /// <summary>
        /// 获取节点的所有输入连接
        /// </summary>
        public IEnumerable<FlowConnection> GetIncomingConnections(Guid nodeId)
        {
            return Connections.Where(c => c.TargetNodeId == nodeId);
        }

        /// <summary>
        /// 获取节点的所有输出连接
        /// </summary>
        public IEnumerable<FlowConnection> GetOutgoingConnections(Guid nodeId)
        {
            return Connections.Where(c => c.SourceNodeId == nodeId);
        }

        /// <summary>
        /// 检查图中是否存在循环依赖
        /// </summary>
        public bool HasCircularDependency()
        {
            var visited = new HashSet<Guid>();
            var recursionStack = new HashSet<Guid>();

            foreach (var node in Nodes)
            {
                if (!visited.Contains(node.Id))
                {
                    if (IsCyclicUtil(node.Id, visited, recursionStack))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 深度优先搜索检测循环
        /// </summary>
        private bool IsCyclicUtil(Guid nodeId, HashSet<Guid> visited, HashSet<Guid> recursionStack)
        {
            visited.Add(nodeId);
            recursionStack.Add(nodeId);

            // 获取所有以当前节点为源的连接
            var outgoingConnections = Connections.Where(c => c.SourceNodeId == nodeId);

            foreach (var connection in outgoingConnections)
            {
                var targetNodeId = connection.TargetNodeId;

                if (!visited.Contains(targetNodeId))
                {
                    if (IsCyclicUtil(targetNodeId, visited, recursionStack))
                    {
                        return true;
                    }
                }
                else if (recursionStack.Contains(targetNodeId))
                {
                    return true;
                }
            }

            recursionStack.Remove(nodeId);
            return false;
        }

        /// <summary>
        /// 获取拓扑排序结果
        /// </summary>
        public List<FlowNode> GetTopologicalSort()
        {
            if (HasCircularDependency())
            {
                throw new InvalidOperationException("图中存在循环依赖，无法进行拓扑排序");
            }

            var result = new List<FlowNode>();
            var visited = new HashSet<Guid>();
            var temp = new HashSet<Guid>();

            foreach (var node in Nodes)
            {
                if (!visited.Contains(node.Id))
                {
                    TopologicalSortUtil(node.Id, visited, temp, result);
                }
            }

            result.Reverse();
            return result;
        }

        /// <summary>
        /// 拓扑排序工具方法
        /// </summary>
        private void TopologicalSortUtil(Guid nodeId, HashSet<Guid> visited, HashSet<Guid> temp, List<FlowNode> result)
        {
            if (temp.Contains(nodeId))
            {
                return; // 已经在处理中
            }

            if (visited.Contains(nodeId))
            {
                return; // 已经访问过
            }

            temp.Add(nodeId);

            // 处理所有依赖的节点
            var outgoingConnections = Connections.Where(c => c.SourceNodeId == nodeId);
            foreach (var connection in outgoingConnections)
            {
                TopologicalSortUtil(connection.TargetNodeId, visited, temp, result);
            }

            temp.Remove(nodeId);
            visited.Add(nodeId);

            var node = GetNode(nodeId);
            if (node != null)
            {
                result.Add(node);
            }
        }

        /// <summary>
        /// 更新验证器
        /// </summary>
        private void UpdateValidator()
        {
            _validator = new ConnectionValidator(Nodes);
        }

    }


}
