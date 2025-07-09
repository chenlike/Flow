using JstFlow.Internal.NodeMeta;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JstFlow.Internal.Utils
{
    /// <summary>
    /// 连接验证器，负责验证连接的有效性
    /// </summary>
    public class ConnectionValidator
    {
        private readonly Dictionary<Guid, FlowNode> _nodeIndex;
        private readonly Dictionary<string, InputField> _inputFieldIndex;
        private readonly Dictionary<string, OutputField> _outputFieldIndex;
        private readonly Dictionary<string, SignalInfo> _signalIndex;
        private readonly Dictionary<string, EmitInfo> _emitIndex;

        public ConnectionValidator(IEnumerable<FlowNode> nodes)
        {
            _nodeIndex = nodes.ToDictionary(n => n.Id);
            _inputFieldIndex = new Dictionary<string, InputField>();
            _outputFieldIndex = new Dictionary<string, OutputField>();
            _signalIndex = new Dictionary<string, SignalInfo>();
            _emitIndex = new Dictionary<string, EmitInfo>();

            BuildFieldIndexes(nodes);
        }

        /// <summary>
        /// 构建字段索引
        /// </summary>
        private void BuildFieldIndexes(IEnumerable<FlowNode> nodes)
        {
            foreach (var node in nodes)
            {
                var nodeId = node.Id.ToString();

                // 索引输入字段
                if (node.InputFields != null)
                {
                    foreach (var field in node.InputFields)
                    {
                        var key = $"{nodeId}:{field.Label.Code}";
                        _inputFieldIndex[key] = field;
                    }
                }

                // 索引输出字段
                if (node.OutputFields != null)
                {
                    foreach (var field in node.OutputFields)
                    {
                        var key = $"{nodeId}:{field.Label.Code}";
                        _outputFieldIndex[key] = field;
                    }
                }

                // 索引信号
                if (node.Signals != null)
                {
                    foreach (var signal in node.Signals)
                    {
                        var key = $"{nodeId}:{signal.Label.Code}";
                        _signalIndex[key] = signal;
                    }
                }

                // 索引事件
                if (node.Emits != null)
                {
                    foreach (var emit in node.Emits)
                    {
                        var key = $"{nodeId}:{emit.Label.Code}";
                        _emitIndex[key] = emit;
                    }
                }
            }
        }

        /// <summary>
        /// 验证连接
        /// </summary>
        public ValidationResult ValidateConnection(FlowConnection connection, IEnumerable<FlowConnection> existingConnections)
        {
            var result = new ValidationResult();

            // 基本验证
            if (!ValidateBasicConnection(connection, result))
                return result;

            // 根据连接类型进行验证
            switch (connection.Type)
            {
                case ConnectionType.OutputToInput:
                    ValidateOutputToInputConnection(connection, result);
                    break;
                case ConnectionType.EventToSignal:
                    ValidateEventToSignalConnection(connection, result);
                    break;
                default:
                    result.AddError($"不支持的连接类型: {connection.Type}");
                    break;
            }

            // 检查重复连接
            if (result.IsValid)
            {
                ValidateDuplicateConnection(connection, existingConnections, result);
            }

            // 检查循环依赖
            if (result.IsValid)
            {
                ValidateCircularDependency(connection, existingConnections, result);
            }

            return result;
        }

        /// <summary>
        /// 基本连接验证
        /// </summary>
        private bool ValidateBasicConnection(FlowConnection connection, ValidationResult result)
        {
            // 验证源节点是否存在
            if (!_nodeIndex.ContainsKey(connection.SourceNodeId))
            {
                result.AddError($"源节点不存在: {connection.SourceNodeId}");
                return false;
            }

            // 验证目标节点是否存在
            if (!_nodeIndex.ContainsKey(connection.TargetNodeId))
            {
                result.AddError($"目标节点不存在: {connection.TargetNodeId}");
                return false;
            }

            // 验证不能连接到自己
            if (connection.SourceNodeId == connection.TargetNodeId)
            {
                result.AddError("不能连接到自己");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 验证输出到输入的连接
        /// </summary>
        private void ValidateOutputToInputConnection(FlowConnection connection, ValidationResult result)
        {
            var sourceKey = $"{connection.SourceNodeId}:{connection.SourceEndpointCode}";
            var targetKey = $"{connection.TargetNodeId}:{connection.TargetEndpointCode}";

            // 验证源输出字段
            if (!_outputFieldIndex.ContainsKey(sourceKey))
            {
                result.AddError($"源节点不存在输出字段: {connection.SourceEndpointCode}");
                return;
            }

            // 验证目标输入字段
            if (!_inputFieldIndex.ContainsKey(targetKey))
            {
                result.AddError($"目标节点不存在输入字段: {connection.TargetEndpointCode}");
                return;
            }

            // 验证类型兼容性
            var sourceField = _outputFieldIndex[sourceKey];
            var targetField = _inputFieldIndex[targetKey];

            if (!IsTypeCompatible(sourceField.Type, targetField.Type))
            {
                result.AddError($"类型不兼容: {sourceField.Type} -> {targetField.Type}");
            }
        }

        /// <summary>
        /// 验证事件到信号的连接
        /// </summary>
        private void ValidateEventToSignalConnection(FlowConnection connection, ValidationResult result)
        {
            var sourceKey = $"{connection.SourceNodeId}:{connection.SourceEndpointCode}";
            var targetKey = $"{connection.TargetNodeId}:{connection.TargetEndpointCode}";

            // 验证源事件
            if (!_emitIndex.ContainsKey(sourceKey))
            {
                result.AddError($"源节点不存在事件: {connection.SourceEndpointCode}");
                return;
            }

            // 验证目标信号
            if (!_signalIndex.ContainsKey(targetKey))
            {
                result.AddError($"目标节点不存在信号: {connection.TargetEndpointCode}");
                return;
            }
        }

        /// <summary>
        /// 验证重复连接
        /// </summary>
        private void ValidateDuplicateConnection(FlowConnection connection, IEnumerable<FlowConnection> existingConnections, ValidationResult result)
        {
            if (existingConnections.Any(c =>
                c.SourceNodeId == connection.SourceNodeId &&
                c.TargetNodeId == connection.TargetNodeId &&
                c.SourceEndpointCode == connection.SourceEndpointCode &&
                c.TargetEndpointCode == connection.TargetEndpointCode))
            {
                result.AddError("相同的连接已存在");
            }
        }

        /// <summary>
        /// 验证循环依赖
        /// </summary>
        private void ValidateCircularDependency(FlowConnection connection, IEnumerable<FlowConnection> existingConnections, ValidationResult result)
        {
            // 创建临时的连接列表进行检测
            var tempConnections = existingConnections.ToList();
            tempConnections.Add(connection);

            if (HasCircularDependency(tempConnections))
            {
                result.AddError("检测到循环依赖，连接将导致无限循环");
            }
        }

        /// <summary>
        /// 检测是否存在循环依赖
        /// </summary>
        private bool HasCircularDependency(List<FlowConnection> connections)
        {
            var visited = new HashSet<Guid>();
            var recursionStack = new HashSet<Guid>();

            foreach (var node in _nodeIndex.Values)
            {
                if (!visited.Contains(node.Id))
                {
                    if (IsCyclicUtil(node.Id, connections, visited, recursionStack))
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
        private bool IsCyclicUtil(Guid nodeId, List<FlowConnection> connections, HashSet<Guid> visited, HashSet<Guid> recursionStack)
        {
            visited.Add(nodeId);
            recursionStack.Add(nodeId);

            // 获取所有以当前节点为源的连接
            var outgoingConnections = connections.Where(c => c.SourceNodeId == nodeId);

            foreach (var connection in outgoingConnections)
            {
                var targetNodeId = connection.TargetNodeId;

                if (!visited.Contains(targetNodeId))
                {
                    if (IsCyclicUtil(targetNodeId, connections, visited, recursionStack))
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
        /// 检查类型兼容性
        /// </summary>
        private bool IsTypeCompatible(string sourceType, string targetType)
        {
            // 如果类型完全相同，则兼容
            if (sourceType == targetType)
                return true;

            // 处理泛型参数的情况
            if (sourceType.Contains("T") && targetType.Contains("T"))
            {
                return true;
            }

            // 处理基本类型的兼容性
            var typeCompatibilityMap = new Dictionary<string, List<string>>
            {
                { "object", new List<string> { "string", "int", "double", "bool", "DateTime" } },
                { "string", new List<string> { "object" } },
                { "int", new List<string> { "double", "object" } },
                { "double", new List<string> { "object" } },
                { "bool", new List<string> { "object" } },
                { "DateTime", new List<string> { "object" } }
            };

            if (typeCompatibilityMap.ContainsKey(sourceType) &&
                typeCompatibilityMap[sourceType].Contains(targetType))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 更新索引（当节点发生变化时调用）
        /// </summary>
        public void UpdateIndexes(IEnumerable<FlowNode> nodes)
        {
            _nodeIndex.Clear();
            _inputFieldIndex.Clear();
            _outputFieldIndex.Clear();
            _signalIndex.Clear();
            _emitIndex.Clear();

            foreach (var node in nodes)
            {
                _nodeIndex[node.Id] = node;
            }

            BuildFieldIndexes(nodes);
        }
    }

    /// <summary>
    /// 验证结果
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid => !Errors.Any();
        public List<string> Errors { get; } = new List<string>();

        public void AddError(string error)
        {
            Errors.Add(error);
        }

        public string GetErrorMessage()
        {
            return string.Join("; ", Errors);
        }
    }
}