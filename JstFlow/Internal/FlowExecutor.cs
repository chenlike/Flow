using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;
using JstFlow.Internal.NodeMeta;
using JstFlow.Internal.Metas;
using System.Threading.Tasks;
using JstFlow.External;

namespace JstFlow.Internal
{
    /// <summary>
    /// 流程图执行器，负责执行FlowGraph中定义的节点和连接
    /// </summary>
    public class FlowExecutor
    {
        /// <summary>
        /// 要执行的流程图
        /// </summary>
        private FlowGraph _flowGraph;

        /// <summary>
        /// 节点实例缓存
        /// </summary>
        private Dictionary<Guid, object> _nodeInstances;

        /// <summary>
        /// 执行状态
        /// </summary>
        public ExecutionStatus Status { get; private set; }

        /// <summary>
        /// 执行错误信息
        /// </summary>
        public List<string> Errors { get; private set; }

        /// <summary>
        /// 执行开始事件
        /// </summary>
        public event Action<FlowExecutor> ExecutionStarted;

        /// <summary>
        /// 执行完成事件
        /// </summary>
        public event Action<FlowExecutor> ExecutionCompleted;

        /// <summary>
        /// 节点执行事件
        /// </summary>
        public event Action<FlowNode, object> NodeExecuted;

        /// <summary>
        /// 执行错误事件
        /// </summary>
        public event Action<FlowNode, Exception> NodeExecutionError;

        /// <summary>
        /// 构造函数
        /// </summary>
        public FlowExecutor(FlowGraph flowGraph)
        {
            _flowGraph = flowGraph ?? throw new ArgumentNullException(nameof(flowGraph));
            _nodeInstances = new Dictionary<Guid, object>();
            Errors = new List<string>();
            Status = ExecutionStatus.UnInitialized;
        }

        public void Initialize()
        {
            if (Status != ExecutionStatus.UnInitialized)
            {
                throw new InvalidOperationException("流程图已初始化");
            }






        }

        private void initializeNodeInstances()
        {
            foreach (var node in _flowGraph.Nodes)
            {
                _nodeInstances[node.Id] = createNodeInstance(node);
            }
        }

        private object createNodeInstance(FlowNode node)
        {
            var nodeType = node.GetType();
            var nodeInstance = Activator.CreateInstance(nodeType);
            return nodeInstance;
        }



    }

    /// <summary>
    /// 执行状态枚举
    /// </summary>
    public enum ExecutionStatus
    {
        /// <summary>
        /// 未初始化
        /// </summary>
        UnInitialized,

        /// <summary>
        /// 准备就绪
        /// </summary>
        Ready,

        /// <summary>
        /// 正在执行
        /// </summary>
        Running,

        /// <summary>
        /// 暂停
        /// </summary>
        Paused,

        /// <summary>
        /// 执行完成
        /// </summary>
        Completed,

        /// <summary>
        /// 执行失败
        /// </summary>
        Failed,
    }
}
