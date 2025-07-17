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
using System.Collections;

namespace JstFlow.Core
{
    /// <summary>
    /// 流程图执行器，负责执行 FlowGraph 中定义的节点和连接
    /// 仅支持 .NET Standard 同步执行
    /// </summary>
    public class FlowExecutor
    {
        private readonly FlowGraph _flowGraph;
        private readonly Dictionary<long, FlowNodeInfo> _nodeMap;
        private readonly Stack<ExecutionTask> _executionStack;
        private readonly Dictionary<long, object> _nodeInstances;
        private readonly Dictionary<long, IEnumerator<FlowOutEvent>> _activeEnumerators;
        private readonly Dictionary<long, Dictionary<string, object>> _nodeInputValues;

        public long CurrentNodeId { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsPaused { get; private set; }

        private FlowExecutor(FlowGraph graph)
        {
            _flowGraph = graph;
            _nodeMap = graph.Nodes.ToDictionary(n => n.Id);
            _executionStack = new Stack<ExecutionTask>();
            _nodeInstances = new Dictionary<long, object>();
            _activeEnumerators = new Dictionary<long, IEnumerator<FlowOutEvent>>();
            _nodeInputValues = new Dictionary<long, Dictionary<string, object>>();
        }

        public static FlowExecutor Create(FlowGraph graph)
        {
            var validate = graph.ValidateGraph();
            if (validate.IsFailure)
                throw new InvalidOperationException(validate.Message);

            return new FlowExecutor(graph);
        }

        public Res Start()
        {
            var startNode = _flowGraph.Nodes.FirstOrDefault(n => n.Kind == NodeKind.StartNode);
            if (startNode == null)
            {
                return Res.Fail("流程图没有启动节点");
            }

            _executionStack.Push(new ExecutionTask(startNode.Id, nameof(StartNode.Start)));

            return Res.Ok();
        }

        public Res StepNext()
        {
            if (_executionStack.Count == 0)
            {
                return Res.Fail("没有可执行的节点");
            }

            var currentTask = _executionStack.Pop();
            var node = _nodeMap[currentTask.NodeId];

            // 节点输入参数
            var inputs = PrepareInputs(currentTask.NodeId);
            _nodeInputValues[currentTask.NodeId] = inputs;

            // 执行当前节点
            var nextTasks = ExecuteNode(currentTask);

            // 将下一个任务推入执行栈
            foreach (var task in nextTasks)
            {
                _executionStack.Push(task);
            }

            return Res.Ok();
        }

        private List<ExecutionTask> ExecuteNode(ExecutionTask task)
        {
            var node = _nodeMap[task.NodeId];
            if (node.Kind == NodeKind.Expression)
                return new List<ExecutionTask>();

            var instance = GetOrCreateNodeInstance(node.Id);
            InjectContext(instance, task.NodeId);

            _nodeInputValues.TryGetValue(task.NodeId, out var inputs);
            if (inputs == null)
                inputs = new Dictionary<string, object>();
            SetInputs(instance, inputs);

            var nextTasks = new List<ExecutionTask>();

            // 根据触发类型处理
            var signal = GetSignal(node.Id, task.TriggerSignal);
            if (signal != null)
                nextTasks.AddRange(ExecuteSignal(instance, signal, inputs, task.NodeId));

            return nextTasks;
        }

        private List<ExecutionTask> ExecuteSignal(object instance, SignalInfo signal, Dictionary<string, object> inputs, long nodeId)
        {
            var nextTasks = new List<ExecutionTask>();

            // 根据返回值处理不同情况
            if (signal.MethodInfo.ReturnType == typeof(void))
            {
                // 无返回值
                signal.MethodInfo.Invoke(instance, null);
            }
            else if (signal.MethodInfo.ReturnType == typeof(FlowOutEvent))
            {
                // 单个事件
                var outEvent = (FlowOutEvent)signal.MethodInfo.Invoke(instance, null);
                var nextTask = GetSignalNextTask(nodeId, outEvent);
                if (nextTask != null)
                {
                    nextTasks.Add(nextTask);
                }
            }
            else if (signal.MethodInfo.ReturnType == typeof(IEnumerable<FlowOutEvent>))
            {
                // 多个事件
                var enumable = (IEnumerable<FlowOutEvent>)signal.MethodInfo.Invoke(instance, null);
                var enumerator = enumable.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    _activeEnumerators[nodeId] = enumerator;
                    var nextTask = GetSignalNextTask(nodeId, enumerator.Current);
                    if (nextTask != null)
                    {
                        nextTasks.Add(nextTask);
                    }
                }
            }
            else if (signal.MethodInfo.ReturnType == typeof(IEnumerator<FlowOutEvent>))
            {
                var enumerator = (IEnumerator<FlowOutEvent>)signal.MethodInfo.Invoke(instance, null);
                if (enumerator.MoveNext())
                {
                    _activeEnumerators[nodeId] = enumerator;
                    var nextTask = GetSignalNextTask(nodeId, enumerator.Current);
                    if (nextTask != null)
                    {
                        nextTasks.Add(nextTask);
                    }
                }
            }


            return nextTasks;
        }

        private List<ExecutionTask> ExecuteEvent(object instance, FlowOutEvent outEvent, Dictionary<string, object> inputs)
        {
            var nextTasks = new List<ExecutionTask>();

            if (_activeEnumerators.TryGetValue(CurrentNodeId, out var enumerator))
            {
                if (enumerator.MoveNext())
                {
                    nextTasks.Add(new ExecutionTask(CurrentNodeId, enumerator.Current));
                }
                else
                {
                    _activeEnumerators.Remove(CurrentNodeId); // 清理
                }
            }

            return nextTasks;
        }


        private ExecutionTask GetSignalNextTask(long nodeId, FlowOutEvent outEvent)
        {
            if (outEvent == null) return null;
            
            // 获取事件对应的端点
            var endpoint = outEvent.MemberName;
            if (endpoint == null) return null;
            
            // 查找从当前节点出发的EventToSignal连接
            var connections = _flowGraph.Connections
                .Where(c => c.SourceNodeId == nodeId && 
                           c.Type == ConnectionType.EventToSignal &&
                           c.SourceEndpointCode == endpoint)
                .ToList();
            
            // 如果没有找到连接，返回null
            if (!connections.Any()) return null;
            
            // 获取第一个连接的目标节点和信号
            var connection = connections.First();
            var targetNodeId = connection.TargetNodeId;
            var signalCode = connection.TargetEndpointCode;
            

            // 返回新的执行任务
            return new ExecutionTask(targetNodeId, signalCode);
        }


        private SignalInfo GetSignal(long nodeId, string triggerSignal)
        {
            var node = _nodeMap[nodeId];
            SignalInfo signal = null;
            if (!string.IsNullOrEmpty(triggerSignal))
            {
                signal = node.Signals.FirstOrDefault(s => s.Label.Code == triggerSignal);
            }
            return signal;
        }

        private void SetInputs(object instance, Dictionary<string, object> inputs)
        {
            if (inputs == null) return;
            var props = instance.GetType().GetProperties();
            foreach (var kv in inputs)
            {
                var prop = props.FirstOrDefault(p => p.Name == kv.Key);
                if (prop != null && kv.Value != null)
                {
                    try { prop.SetValue(instance, Convert.ChangeType(kv.Value, prop.PropertyType)); }
                    catch { }
                }
            }
        }

        private void InjectContext(object instance, long nodeId)
        {
            if (instance is FlowBaseNode baseNode)
            {
                var ctx = new NodeContext { Executor = this, FlowGraph = _flowGraph, NodeMap = _nodeMap, CurrentNodeId = nodeId };
                baseNode.Inject(ctx);
            }
        }

        private object GetOrCreateNodeInstance(long nodeId)
        {
            if (!_nodeInstances.TryGetValue(nodeId, out var nodeInstance))
            {
                var node = _nodeMap[nodeId];
                nodeInstance = Activator.CreateInstance(node.NodeImplType);
                _nodeInstances[nodeId] = nodeInstance;
            }
            return nodeInstance;
        }

        private Dictionary<string, object> PrepareInputs(long nodeId)
        {
            var inputs = new Dictionary<string, object>();
            var connections = _flowGraph.Connections
                .Where(c => c.TargetNodeId == nodeId && c.Type == ConnectionType.OutputToInput);
            foreach (var conn in connections)
            {
                object val = _nodeMap[conn.SourceNodeId].Kind == NodeKind.Expression
                    ? ExecuteExpression(conn.SourceNodeId)
                    : GetOutputValue(conn.SourceNodeId, conn.SourceEndpointCode);
                if (val != null) inputs[conn.TargetEndpointCode] = val;
            }
            return inputs;
        }

        private object ExecuteExpression(long exprId)
        {
            var node = _nodeMap[exprId];
            var inst = Activator.CreateInstance(node.NodeImplType);
            var mi = inst.GetType().GetMethod(nameof(FlowExpression<object>.Evaluate));
            return mi?.Invoke(inst, null);
        }

        private object GetOutputValue(long nodeId, string code)
        {
            var node = _nodeMap[nodeId];
            var inst = GetOrCreateNodeInstance(nodeId);
            var field = node.OutputFields.FirstOrDefault(f => f.Label.Code == code);
            return field?.PropertyInfo.GetValue(inst);
        }

        /// <summary>
        /// 执行任务类，支持事件和信号两种触发方式
        /// </summary>
        public class ExecutionTask
        {
            public long NodeId { get; }
            public string TriggerSignal { get; }

            /// <summary>
            /// 通过信号触发的构造函数
            /// </summary>
            public ExecutionTask(long nodeId, string triggerSignal = null)
            {
                NodeId = nodeId;
                TriggerSignal = triggerSignal;
            }
        }
    }
}