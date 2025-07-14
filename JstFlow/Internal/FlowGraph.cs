using JstFlow.Internal.NodeMeta;
using JstFlow.Internal.Metas;
using System;
using System.Collections.Generic;
using System.Text;
using JstFlow.Common;
using System.Linq;
using JstFlow.Internal.Utils;
using JstFlow.External;

namespace JstFlow.Internal
{
    /// <summary>
    /// 流程图，包含节点和连接
    /// </summary>
    public partial class FlowGraph
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

            // 开始节点只能有一个
            if (node.GetType() == typeof(StartNode))
            {
                if (Nodes.Any(n => n.GetType() == typeof(StartNode)))
                {
                    throw new ArgumentException("只能有一个开始节点", nameof(node));
                }
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
        /// 更新验证器
        /// </summary>
        private void UpdateValidator()
        {
            _validator = new ConnectionValidator(Nodes);
        }

    }


}
