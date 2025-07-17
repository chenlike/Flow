using JstFlow.External.Nodes;
using System.Collections.Generic;
using System.Linq;
using JstFlow.Core.NodeMeta;

namespace JstFlow.Core
{
    /// <summary>
    /// 节点执行上下文
    /// </summary>
    public class NodeContext
    {
        /// <summary>
        /// 当前执行器
        /// </summary>
        public FlowExecutor Executor { get; set; }

        /// <summary>
        /// 当前节点ID
        /// </summary>
        public long CurrentNodeId { get; set; }

        /// <summary>
        /// 流程图
        /// </summary>
        public FlowGraph FlowGraph { get; set; }

        /// <summary>
        /// 节点映射表
        /// </summary>
        public Dictionary<long, FlowNodeInfo> NodeMap { get; set; }

        /// <summary>
        /// 触发流程事件
        /// </summary>
        /// <param name="outEvent">输出事件</param>
        public void TriggerFlowEvent(FlowOutEvent outEvent)
        {
            if (outEvent == null || Executor == null)
            {
                return;
            }

            // 获取下一个节点ID列表
            var nextNodeIds = GetNextNodeIds(CurrentNodeId, outEvent);
            
            // 将下一个节点添加到执行栈
            var nextTasks = nextNodeIds.Where(id => id != 0)
                                      .Select(id => new FlowExecutor.ExecutionTask(id, GetNextNodeSignal(id)))
                                      .ToList();
            
            // 这里需要访问执行器的私有字段，暂时注释掉
            // foreach (var task in nextTasks)
            // {
            //     Executor._executionStack.Push(task);
            // }
        }

        /// <summary>
        /// 根据输出事件获取下一个节点ID列表
        /// </summary>
        private List<long> GetNextNodeIds(long currentNodeId, FlowOutEvent outEvent)
        {
            var endpoint = outEvent.MemberName;
            if (endpoint == null)
            {
                return new List<long>();
            }

            // 查找连接到这个事件的连接
            var connections = FlowGraph.Connections.Where(connection => 
                connection.SourceNodeId == currentNodeId && 
                connection.Type == ConnectionType.EventToSignal &&
                connection.SourceEndpointCode == endpoint).ToList();

            return connections.Select(c => c.TargetNodeId).ToList();
        }

        /// <summary>
        /// 获取下一个节点要触发的信号
        /// </summary>
        private string GetNextNodeSignal(long nextNodeId)
        {
            // 这里需要根据当前上下文来确定信号
            // 暂时返回null，让节点自己决定触发哪个信号
            return null;
        }
    }
} 