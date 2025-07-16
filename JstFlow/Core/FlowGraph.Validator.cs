using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JstFlow.Common;
using JstFlow.Core.Metas;

namespace JstFlow.Core
{
    public partial class FlowGraph
    {

        internal Res ValidateGraph()
        {
            if (Nodes == null || Nodes.Count == 0)
            {
                return Res.Fail("图中没有节点");
            }

            // 先检查空节点
            foreach (var node in Nodes)
            {
                if (node == null)
                {
                    return Res.Fail("图中存在空节点");
                }
            }

            // 检查必须有StartNode
            var startNodes = Nodes.Where(n => n.Kind == NodeKind.StartNode).ToList();
            if (startNodes.Count == 0)
            {
                return Res.Fail("图中必须包含至少一个开始节点");
            }
            if (startNodes.Count > 1)
            {
                return Res.Fail("图中只能有一个开始节点");
            }

            // 检查节点ID重复
            var nodeIds = new HashSet<long>();
            foreach (var node in Nodes)
            {

                if (nodeIds.Contains(node.Id))
                {
                    return Res.Fail($"节点ID重复: {node.Id}");
                }
                nodeIds.Add(node.Id);

                // 验证节点基本信息
                if (node.NodeImplType == null)
                {
                    return Res.Fail($"节点 {node.Id} 缺少实现类型");
                }

                if (node.Label == null)
                {
                    return Res.Fail($"节点 {node.Id} 缺少标签");
                }

                // 验证输入字段
                if (node.InputFields != null)
                {
                    var inputFieldCodes = new HashSet<string>();
                    foreach (var inputField in node.InputFields)
                    {
                        if (inputField == null)
                        {
                            return Res.Fail($"节点 {node.Id} 存在空输入字段");
                        }

                        if (string.IsNullOrEmpty(inputField.Label?.Code))
                        {
                            return Res.Fail($"节点 {node.Id} 输入字段缺少代码");
                        }

                        if (inputFieldCodes.Contains(inputField.Label.Code))
                        {
                            return Res.Fail($"节点 {node.Id} 输入字段代码重复: {inputField.Label.Code}");
                        }
                        inputFieldCodes.Add(inputField.Label.Code);
                    }
                }

                // 验证输出字段
                if (node.OutputFields != null)
                {
                    var outputFieldCodes = new HashSet<string>();
                    foreach (var outputField in node.OutputFields)
                    {
                        if (outputField == null)
                        {
                            return Res.Fail($"节点 {node.Id} 存在空输出字段");
                        }

                        if (string.IsNullOrEmpty(outputField.Label?.Code))
                        {
                            return Res.Fail($"节点 {node.Id} 输出字段缺少代码");
                        }

                        if (outputFieldCodes.Contains(outputField.Label.Code))
                        {
                            return Res.Fail($"节点 {node.Id} 输出字段代码重复: {outputField.Label.Code}");
                        }
                        outputFieldCodes.Add(outputField.Label.Code);
                    }
                }

                // 验证信号
                if (node.Signals != null)
                {
                    var signalCodes = new HashSet<string>();
                    foreach (var signal in node.Signals)
                    {
                        if (signal == null)
                        {
                            return Res.Fail($"节点 {node.Id} 存在空信号");
                        }

                        if (string.IsNullOrEmpty(signal.Label?.Code))
                        {
                            return Res.Fail($"节点 {node.Id} 信号缺少代码");
                        }

                        if (signalCodes.Contains(signal.Label.Code))
                        {
                            return Res.Fail($"节点 {node.Id} 信号代码重复: {signal.Label.Code}");
                        }
                        signalCodes.Add(signal.Label.Code);
                    }
                }

                // 验证事件
                if (node.Emits != null)
                {
                    var emitCodes = new HashSet<string>();
                    foreach (var emit in node.Emits)
                    {
                        if (emit == null)
                        {
                            return Res.Fail($"节点 {node.Id} 存在空事件");
                        }

                        if (string.IsNullOrEmpty(emit.Label?.Code))
                        {
                            return Res.Fail($"节点 {node.Id} 事件缺少代码");
                        }

                        if (emitCodes.Contains(emit.Label.Code))
                        {
                            return Res.Fail($"节点 {node.Id} 事件代码重复: {emit.Label.Code}");
                        }
                        emitCodes.Add(emit.Label.Code);
                    }
                }
            }

            // 验证连接
            if (Connections != null)
            {
                var connectionIds = new HashSet<long>();
                var nodeDict = Nodes.ToDictionary(n => n.Id);

                foreach (var connection in Connections)
                {
                    if (connection == null)
                    {
                        return Res.Fail("图中存在空连接");
                    }

                    // 检查连接ID重复
                    if (connectionIds.Contains(connection.Id))
                    {
                        return Res.Fail($"连接ID重复: {connection.Id}");
                    }
                    connectionIds.Add(connection.Id);

                    // 验证源节点存在
                    if (!nodeDict.ContainsKey(connection.SourceNodeId))
                    {
                        return Res.Fail($"连接 {connection.Id} 的源节点 {connection.SourceNodeId} 不存在");
                    }

                    // 验证目标节点存在
                    if (!nodeDict.ContainsKey(connection.TargetNodeId))
                    {
                        return Res.Fail($"连接 {connection.Id} 的目标节点 {connection.TargetNodeId} 不存在");
                    }

                    var sourceNode = nodeDict[connection.SourceNodeId];
                    var targetNode = nodeDict[connection.TargetNodeId];

                    // 检查是否存在环路（简单检查）
                    if (connection.SourceNodeId == connection.TargetNodeId)
                    {
                        return Res.Fail($"连接 {connection.Id} 不能连接到自身节点");
                    }

                    // 验证端点存在
                    if (connection.Type == ConnectionType.OutputToInput)
                    {
                        // 验证源节点输出字段存在
                        if (sourceNode.OutputFields == null || 
                            !sourceNode.OutputFields.Any(f => f.Label?.Code == connection.SourceEndpointCode))
                        {
                            return Res.Fail($"连接 {connection.Id} 的源节点 {connection.SourceNodeId} 不存在输出字段 {connection.SourceEndpointCode}");
                        }

                        // 验证目标节点输入字段存在
                        if (targetNode.InputFields == null || 
                            !targetNode.InputFields.Any(f => f.Label?.Code == connection.TargetEndpointCode))
                        {
                            return Res.Fail($"连接 {connection.Id} 的目标节点 {connection.TargetNodeId} 不存在输入字段 {connection.TargetEndpointCode}");
                        }
                    }
                    else if (connection.Type == ConnectionType.EventToSignal)
                    {
                        // 验证源节点事件存在
                        if (sourceNode.Emits == null || 
                            !sourceNode.Emits.Any(e => e.Label?.Code == connection.SourceEndpointCode))
                        {
                            return Res.Fail($"连接 {connection.Id} 的源节点 {connection.SourceNodeId} 不存在事件 {connection.SourceEndpointCode}");
                        }

                        // 验证目标节点信号存在
                        if (targetNode.Signals == null || 
                            !targetNode.Signals.Any(s => s.Label?.Code == connection.TargetEndpointCode))
                        {
                            return Res.Fail($"连接 {connection.Id} 的目标节点 {connection.TargetNodeId} 不存在信号 {connection.TargetEndpointCode}");
                        }
                    }
                }

                // 检查一个事件只能连接一个信号
                var eventConnections = Connections.Where(c => c.Type == ConnectionType.EventToSignal)
                    .GroupBy(c => new { c.SourceNodeId, c.SourceEndpointCode })
                    .ToList();

                foreach (var eventGroup in eventConnections)
                {
                    if (eventGroup.Count() > 1)
                    {
                        return Res.Fail($"节点 {eventGroup.Key.SourceNodeId} 的事件 {eventGroup.Key.SourceEndpointCode} 连接了多个信号，一个事件只能连接一个信号");
                    }
                }

                // 检查必填输入字段是否都有连接
                foreach (var node in Nodes)
                {
                    if (node.InputFields != null)
                    {
                        var requiredFields = node.InputFields.Where(f => f.Required).ToList();
                        foreach (var requiredField in requiredFields)
                        {
                            var hasConnection = Connections.Any(c => 
                                c.Type == ConnectionType.OutputToInput &&
                                c.TargetNodeId == node.Id &&
                                c.TargetEndpointCode == requiredField.Label.Code);

                            if (!hasConnection)
                            {
                                return Res.Fail($"节点 {node.Id} 的必填输入字段 {requiredField.Label.Code} 没有连接");
                            }
                        }
                    }
                }
            }

            return Res.Ok("图验证通过");
        }



    }
}
