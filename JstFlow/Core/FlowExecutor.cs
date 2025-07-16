using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;
using JstFlow.Core.NodeMeta;
using JstFlow.Core.Metas;
using System.Threading.Tasks;
using JstFlow.External;
using JstFlow.Common;
using JstFlow.External.Nodes;
using JstFlow.External.Expressions;
using static JstFlow.Core.FlowExecutor;

namespace JstFlow.Core
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
        /// 节点映射表
        /// </summary>
        private Dictionary<long, FlowNodeInfo> _nodeMap;

        /// <summary>
        /// 当前执行的节点ID
        /// </summary>
        public long CurrentNodeId { get; private set; }

        /// <summary>
        /// 执行状态
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// 是否暂停
        /// </summary>
        public bool IsPaused { get; private set; }

        /// <summary>
        /// 节点实例缓存
        /// </summary>
        private Dictionary<long, object> _nodeInstances;

        /// <summary>
        /// 表达式实例缓存
        /// </summary>
        private Dictionary<long, object> _expressionInstances;

        /// <summary>
        /// 节点上下文
        /// </summary>
        private NodeContext _nodeContext;

        /// <summary>
        /// 执行历史
        /// </summary>
        private List<long> _executionHistory;

        /// <summary>
        /// 待执行节点栈
        /// </summary>
        internal Stack<ExecutionTask> _executionStack;

        /// <summary>
        /// 执行任务
        /// </summary>
        public class ExecutionTask
        {
            /// <summary>
            /// 节点ID
            /// </summary>
            public long NodeId { get; set; }

            /// <summary>
            /// 输入参数
            /// </summary>
            public Dictionary<string, object> InputValues { get; set; }

            /// <summary>
            /// 任务优先级（用于条件分支等）
            /// </summary>
            public int Priority { get; set; }

            /// <summary>
            /// 要触发的信号名称（如果为空，则触发所有可用信号）
            /// </summary>
            public string TriggerSignal { get; set; }

            public ExecutionTask(long nodeId, Dictionary<string, object> inputValues = null, int priority = 0, string triggerSignal = null)
            {
                NodeId = nodeId;
                InputValues = inputValues ?? new Dictionary<string, object>();
                Priority = priority;
                TriggerSignal = triggerSignal;
            }
        }

        private FlowExecutor(){
            _nodeInstances = new Dictionary<long, object>();
            _expressionInstances = new Dictionary<long, object>();
            _nodeContext = new NodeContext();
            _executionHistory = new List<long>();
            _executionStack = new Stack<ExecutionTask>();
        }

        public static Res<FlowExecutor> Create(FlowGraph flowGraph){
            var executor = new FlowExecutor();
            executor._flowGraph = flowGraph;
            var validateRes = flowGraph.ValidateGraph();
            if (validateRes.IsFailure)
            {
                return Res<FlowExecutor>.Fail(validateRes.Message);
            }
            executor._nodeMap = flowGraph.Nodes.ToDictionary(node => node.Id, node => node);
            
            // 设置NodeContext的属性
            executor._nodeContext.Executor = executor;
            executor._nodeContext.FlowGraph = flowGraph;
            executor._nodeContext.NodeMap = executor._nodeMap;
            
            return Res<FlowExecutor>.Ok(executor, validateRes.Message);
        }

        /// <summary>
        /// 开始执行流程图
        /// </summary>
        public void Start(){
            if (IsRunning)
            {
                throw new InvalidOperationException("流程图已在执行中");
            }

            var startNode = _flowGraph.Nodes.FirstOrDefault(node => node.Kind == NodeKind.StartNode);
            if (startNode == null)
            {
                throw new InvalidOperationException("流程图必须包含一个开始节点");
            }

            // 清空栈和历史
            _executionStack.Clear();
            _executionHistory.Clear();
            
            IsRunning = true;
            IsPaused = false;
            
            // 先执行StartNode的Execute信号来触发流程
            ExecuteStartNode(startNode.Id);
        }

        /// <summary>
        /// 执行下一个节点
        /// </summary>
        public void ExecuteNext()
        {
            if (!IsRunning || IsPaused)
            {
                return;
            }

            if (_executionStack.Count == 0)
            {
                IsRunning = false;
                return;
            }

            var task = _executionStack.Pop();
            CurrentNodeId = task.NodeId;
            _executionHistory.Add(task.NodeId);

            // 准备节点输入（如果没有提供的话）
            if (task.InputValues.Count == 0)
            {
                task.InputValues = PrepareNode(task.NodeId);
            }

            // 执行节点
            var nextTasks = ExecuteNode(task.NodeId, task.InputValues, task.TriggerSignal);
            
            // 将下一个任务加入执行栈
            PushTasksToStack(nextTasks);

            // 不再自动继续执行，等待外部调用
            if (_executionStack.Count == 0)
            {
                IsRunning = false;
            }
        }

        /// <summary>
        /// 暂停执行
        /// </summary>
        public void Pause()
        {
            IsPaused = true;
        }

        /// <summary>
        /// 恢复执行
        /// </summary>
        public void Resume()
        {
            if (IsPaused)
            {
                IsPaused = false;
                if (IsRunning && _executionStack.Count > 0)
                {
                    ExecuteNext();
                }
            }
        }

        /// <summary>
        /// 手动执行下一个节点
        /// </summary>
        public bool StepNext()
        {
            if (!IsRunning || IsPaused)
            {
                return false;
            }

            if (_executionStack.Count == 0)
            {
                IsRunning = false;
                return false;
            }

            ExecuteNext();
            return true;
        }

        /// <summary>
        /// 执行所有剩余节点
        /// </summary>
        public void ExecuteAll()
        {
            while (IsRunning && !IsPaused && _executionStack.Count > 0)
            {
                ExecuteNext();
            }
        }

        /// <summary>
        /// 检查是否可以继续执行
        /// </summary>
        public bool CanContinue()
        {
            return IsRunning && !IsPaused && _executionStack.Count > 0;
        }

        /// <summary>
        /// 将任务推入执行栈
        /// </summary>
        /// <param name="tasks">要推入的任务列表</param>
        internal void PushTasksToStack(List<ExecutionTask> tasks)
        {
            if (tasks == null || tasks.Count == 0)
            {
                return;
            }

            // 注意顺序：后进先出，所以需要倒序推入
            for (int i = tasks.Count - 1; i >= 0; i--)
            {
                _executionStack.Push(tasks[i]);
            }
        }

        /// <summary>
        /// 将单个任务推入执行栈
        /// </summary>
        /// <param name="task">要推入的任务</param>
        internal void PushTaskToStack(ExecutionTask task)
        {
            if (task != null)
            {
                _executionStack.Push(task);
            }
        }

        /// <summary>
        /// 将节点ID推入执行栈
        /// </summary>
        /// <param name="nodeId">要执行的节点ID</param>
        internal void PushNodeToStack(long nodeId)
        {
            if (nodeId != 0)
            {
                _executionStack.Push(new ExecutionTask(nodeId));
            }
        }

        /// <summary>
        /// 准备节点输入参数
        /// </summary>
        private Dictionary<string, object> PrepareNode(long nodeId)
        {
            var node = _nodeMap[nodeId];
            var nodeConnections = _flowGraph.Connections.Where(connection => connection.TargetNodeId == nodeId).ToList();
            Dictionary<string, object> inputValues = new Dictionary<string, object>();

            // 处理输入字段
            foreach (InputField input in node.InputFields)
            {
                // 查找连接到这个输入字段的连接
                var fieldConnection = nodeConnections.FirstOrDefault(connection => 
                    connection.Type == ConnectionType.OutputToInput && 
                    connection.TargetEndpointCode == input.Label.Code);
                
                if (fieldConnection == null)
                {
                    // 这个字段没有连线，使用默认值
                    continue;
                }

                var sourceNodeId = fieldConnection.SourceNodeId;
                var sourceNode = _nodeMap[sourceNodeId];
                var outputField = sourceNode.OutputFields.FirstOrDefault(field => 
                    field.Label.Code == fieldConnection.SourceEndpointCode);

                if (outputField == null)
                {
                    continue;
                }

                // 获取源节点的输出值
                object outputValue = null;
                
                if (sourceNode.Kind == NodeKind.Expression)
                {
                    // 如果是表达式节点，执行表达式获取值
                    outputValue = ExecuteExpression(sourceNodeId);
                }
                else
                {
                    // 如果是普通节点，从输出字段获取值
                    var nodeInstance = GetOrCreateNodeInstance(sourceNodeId);
                    if (nodeInstance != null)
                    {
                        outputValue = outputField.PropertyInfo.GetValue(nodeInstance);
                    }
                }

                if (outputValue != null)
                {
                    inputValues[input.Label.Code] = outputValue;
                }
            }

            return inputValues;
        }

        /// <summary>
        /// 执行节点
        /// </summary>
        private List<ExecutionTask> ExecuteNode(long nodeId, Dictionary<string, object> inputValues, string triggerSignal = null)
        {
            var node = _nodeMap[nodeId];
            var nextTasks = new List<ExecutionTask>();
            
            if (node.Kind == NodeKind.Expression)
            {
                // 表达式节点不需要执行，只返回空列表
                return nextTasks;
            }

            // 创建或获取节点实例
            var nodeInstance = GetOrCreateNodeInstance(nodeId);
            if (nodeInstance == null)
            {
                return nextTasks;
            }

            // 注入上下文
            if (nodeInstance is FlowBaseNode baseNode)
            {
                _nodeContext.CurrentNodeId = nodeId;
                baseNode.Inject(_nodeContext);
            }

            // 设置输入参数
            foreach (var input in node.InputFields)
            {
                if (inputValues.ContainsKey(input.Label.Code))
                {
                    var value = inputValues[input.Label.Code];
                    // 尝试类型转换
                    try
                    {
                        var convertedValue = Convert.ChangeType(value, input.PropertyInfo.PropertyType);
                        input.PropertyInfo.SetValue(nodeInstance, convertedValue);
                    }
                    catch
                    {
                        // 类型转换失败，跳过
                    }
                }
            }

            // 根据指定的信号或连线确定要触发的信号
            var triggeredSignals = GetTriggeredSignals(nodeId, triggerSignal);
            foreach (var signalInfo in triggeredSignals)
            {
                var result = signalInfo.MethodInfo.Invoke(nodeInstance, null);
                
                if (result is FlowOutEvent outEvent)
                {
                    // 获取下一个节点
                    var nextNodeIds = GetNextNodeIds(nodeId, outEvent);
                    foreach (var nextNodeId in nextNodeIds)
                    {
                        if (nextNodeId != 0)
                        {
                            // 根据连线确定下一个节点要触发的信号
                            var nextSignal = GetNextNodeSignal(nodeId, nextNodeId, outEvent);
                            nextTasks.Add(new ExecutionTask(nextNodeId, null, 0, nextSignal));
                        }
                    }
                }
            }

            return nextTasks;
        }

        /// <summary>
        /// 执行表达式
        /// </summary>
        private object ExecuteExpression(long expressionId)
        {
            object expression;
            if (!_expressionInstances.TryGetValue(expressionId, out expression))
            {
                var expressionInfo = _nodeMap[expressionId];
                expression = Activator.CreateInstance(expressionInfo.NodeImplType);
                _expressionInstances[expressionId] = expression;
            }

            // 调用Evaluate方法
            var evaluateMethod = expression.GetType().GetMethod("Evaluate");
            if (evaluateMethod != null)
            {
                return evaluateMethod.Invoke(expression, null);
            }

            return null;
        }

        /// <summary>
        /// 获取或创建节点实例
        /// </summary>
        private object GetOrCreateNodeInstance(long nodeId)
        {
            if (_nodeInstances.ContainsKey(nodeId))
            {
                return _nodeInstances[nodeId];
            }

            var node = _nodeMap[nodeId];
            try
            {
                var instance = Activator.CreateInstance(node.NodeImplType);
                _nodeInstances[nodeId] = instance;
                return instance;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 执行开始节点
        /// </summary>
        private void ExecuteStartNode(long startNodeId)
        {
            var startNode = _nodeMap[startNodeId];
            
            // 创建开始节点实例
            var nodeInstance = GetOrCreateNodeInstance(startNodeId);
            if (nodeInstance == null)
            {
                return;
            }

            // 注入上下文
            if (nodeInstance is FlowBaseNode baseNode)
            {
                _nodeContext.CurrentNodeId = startNodeId;
                baseNode.Inject(_nodeContext);
            }

            // 查找并执行StartNode的信号（通常是"开始循环"或第一个信号）
            var executeSignal = startNode.Signals.FirstOrDefault();
            if (executeSignal != null)
            {
                // 记录开始节点的执行
                _executionHistory.Add(startNodeId);
                CurrentNodeId = startNodeId;
            }

            if (executeSignal != null)
            {
                var result = executeSignal.MethodInfo.Invoke(nodeInstance, null);
                
                if (result is FlowOutEvent outEvent)
                {
                    // 获取下一个节点
                    var nextNodeIds = GetNextNodeIds(startNodeId, outEvent);
                    var nextTasks = nextNodeIds.Where(id => id != 0)
                                              .Select(id => new ExecutionTask(id, null, 0, GetNextNodeSignal(startNodeId, id, outEvent)))
                                              .ToList();
                    
                    // 将下一个任务加入执行栈
                    PushTasksToStack(nextTasks);
                }
            }

            // 不自动继续执行，等待外部调用
            if (_executionStack.Count == 0)
            {
                IsRunning = false;
            }
        }

        /// <summary>
        /// 根据连线确定要触发的信号
        /// </summary>
        private List<SignalInfo> GetTriggeredSignals(long nodeId, string triggerSignal = null)
        {
            var node = _nodeMap[nodeId];
            var triggeredSignals = new List<SignalInfo>();

            if (!string.IsNullOrEmpty(triggerSignal))
            {
                // 如果指定了信号，直接查找该信号
                var signal = node.Signals.FirstOrDefault(s => s.Label.Code == triggerSignal);
                if (signal != null)
                {
                    triggeredSignals.Add(signal);
                }
            }
            else
            {
                // 查找所有连接到这个节点的信号连接
                var signalConnections = _flowGraph.Connections.Where(connection => 
                    connection.TargetNodeId == nodeId && 
                    connection.Type == ConnectionType.EventToSignal).ToList();

                foreach (var connection in signalConnections)
                {
                    // 查找对应的信号
                    var signal = node.Signals.FirstOrDefault(s => s.Label.Code == connection.TargetEndpointCode);
                    if (signal != null)
                    {
                        triggeredSignals.Add(signal);
                    }
                }

                // 如果没有找到任何信号连接，则触发默认信号（第一个信号）
                if (triggeredSignals.Count == 0 && node.Signals.Count > 0)
                {
                    triggeredSignals.Add(node.Signals.First());
                }
            }

            return triggeredSignals;
        }

        /// <summary>
        /// 获取下一个节点要触发的信号
        /// </summary>
        private string GetNextNodeSignal(long currentNodeId, long nextNodeId, FlowOutEvent outEvent)
        {
            var endpoint = outEvent.Invoke();
            if (endpoint == null)
            {
                return null;
            }

            // 查找从当前节点到下一个节点的连接
            var connection = _flowGraph.Connections.FirstOrDefault(c => 
                c.SourceNodeId == currentNodeId && 
                c.TargetNodeId == nextNodeId && 
                c.Type == ConnectionType.EventToSignal &&
                c.SourceEndpointCode == endpoint.GetType().Name);

            return connection?.TargetEndpointCode;
        }

        /// <summary>
        /// 根据输出事件获取下一个节点ID列表
        /// </summary>
        private List<long> GetNextNodeIds(long currentNodeId, FlowOutEvent outEvent)
        {
            var endpoint = outEvent.Invoke();
            if (endpoint == null)
            {
                return new List<long>();
            }

            // 查找连接到这个事件的连接
            var connections = _flowGraph.Connections.Where(connection => 
                connection.SourceNodeId == currentNodeId && 
                connection.Type == ConnectionType.EventToSignal &&
                connection.SourceEndpointCode == endpoint.GetType().Name).ToList();

            return connections.Select(c => c.TargetNodeId).ToList();
        }

        /// <summary>
        /// 停止执行
        /// </summary>
        public void Stop()
        {
            IsRunning = false;
            IsPaused = false;
            CurrentNodeId = 0;
            _executionStack.Clear();
        }

        /// <summary>
        /// 获取执行历史
        /// </summary>
        public List<long> GetExecutionHistory()
        {
            return new List<long>(_executionHistory);
        }

        /// <summary>
        /// 重置执行器
        /// </summary>
        public void Reset()
        {
            Stop();
            _nodeInstances.Clear();
            _expressionInstances.Clear();
            _executionHistory.Clear();
            _executionStack.Clear();
        }

        /// <summary>
        /// 获取当前节点信息
        /// </summary>
        public FlowNodeInfo GetCurrentNode()
        {
            if (CurrentNodeId == 0 || !_nodeMap.ContainsKey(CurrentNodeId))
            {
                return null;
            }
            return _nodeMap[CurrentNodeId];
        }

        /// <summary>
        /// 获取节点实例
        /// </summary>
        public object GetNodeInstance(long nodeId)
        {
            object instance;
            _nodeInstances.TryGetValue(nodeId, out instance);
            return instance;
        }

        /// <summary>
        /// 获取表达式实例
        /// </summary>
        public object GetExpressionInstance(long expressionId)
        {
            object instance;
            _expressionInstances.TryGetValue(expressionId, out instance);
            return instance;
        }

        /// <summary>
        /// 检查流程图是否已完成执行
        /// </summary>
        public bool IsCompleted()
        {
            return !IsRunning && _executionStack.Count == 0;
        }

        /// <summary>
        /// 获取栈中待执行的任务数量
        /// </summary>
        public int GetPendingTaskCount()
        {
            return _executionStack.Count;
        }

        /// <summary>
        /// 获取执行统计信息
        /// </summary>
        public ExecutionStatistics GetExecutionStatistics()
        {
            return new ExecutionStatistics
            {
                TotalNodesExecuted = _executionHistory.Count,
                ExecutionHistory = new List<long>(_executionHistory),
                IsRunning = IsRunning,
                IsPaused = IsPaused,
                IsCompleted = IsCompleted(),
                CurrentNodeId = CurrentNodeId,
                PendingTaskCount = _executionStack.Count,
                CanContinue = CanContinue()
            };
        }

        /// <summary>
        /// 获取执行快照（用于保存和恢复执行状态）
        /// </summary>
        public ExecutionSnapshot GetSnapshot()
        {
            return new ExecutionSnapshot
            {
                CurrentNodeId = CurrentNodeId,
                IsRunning = IsRunning,
                IsPaused = IsPaused,
                ExecutionHistory = new List<long>(_executionHistory),
                PendingTasks = new List<ExecutionTask>(_executionStack),
                NodeInstances = new Dictionary<long, object>(_nodeInstances),
                ExpressionInstances = new Dictionary<long, object>(_expressionInstances)
            };
        }

        /// <summary>
        /// 从快照恢复执行状态
        /// </summary>
        public void RestoreFromSnapshot(ExecutionSnapshot snapshot)
        {
            if (snapshot == null)
            {
                return;
            }

            CurrentNodeId = snapshot.CurrentNodeId;
            IsRunning = snapshot.IsRunning;
            IsPaused = snapshot.IsPaused;
            _executionHistory = new List<long>(snapshot.ExecutionHistory);
            _executionStack = new Stack<ExecutionTask>(snapshot.PendingTasks);
            _nodeInstances = new Dictionary<long, object>(snapshot.NodeInstances);
            _expressionInstances = new Dictionary<long, object>(snapshot.ExpressionInstances);
        }
    }

    /// <summary>
    /// 执行统计信息
    /// </summary>
    public class ExecutionStatistics
    {
        /// <summary>
        /// 已执行节点总数
        /// </summary>
        public int TotalNodesExecuted { get; set; }

        /// <summary>
        /// 执行历史
        /// </summary>
        public List<long> ExecutionHistory { get; set; }

        /// <summary>
        /// 是否正在运行
        /// </summary>
        public bool IsRunning { get; set; }

        /// <summary>
        /// 是否暂停
        /// </summary>
        public bool IsPaused { get; set; }

        /// <summary>
        /// 是否已完成
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// 当前节点ID
        /// </summary>
        public long CurrentNodeId { get; set; }

        /// <summary>
        /// 待执行任务数量
        /// </summary>
        public int PendingTaskCount { get; set; }

        /// <summary>
        /// 是否可以继续执行
        /// </summary>
        public bool CanContinue { get; set; }
    }

    /// <summary>
    /// 执行快照
    /// </summary>
    public class ExecutionSnapshot
    {
        /// <summary>
        /// 当前节点ID
        /// </summary>
        public long CurrentNodeId { get; set; }

        /// <summary>
        /// 是否正在运行
        /// </summary>
        public bool IsRunning { get; set; }

        /// <summary>
        /// 是否暂停
        /// </summary>
        public bool IsPaused { get; set; }

        /// <summary>
        /// 执行历史
        /// </summary>
        public List<long> ExecutionHistory { get; set; }

        /// <summary>
        /// 待执行任务
        /// </summary>
        public List<ExecutionTask> PendingTasks { get; set; }

        /// <summary>
        /// 节点实例
        /// </summary>
        public Dictionary<long, object> NodeInstances { get; set; }

        /// <summary>
        /// 表达式实例
        /// </summary>
        public Dictionary<long, object> ExpressionInstances { get; set; }
    }
}
