using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;
using JstFlow.Core.NodeMeta;
using JstFlow.Attributes;
using JstFlow.Common;
using JstFlow.Core.Metas;
using JstFlow.External;
using JstFlow.External.Nodes;


namespace JstFlow.Core
{
    public class NodeFactory
    {
        public static Res<FlowNodeInfo> CreateNodeInfo(Type nodeType)
        {

            // 检查null
            if (nodeType == null)
            {
                return Res<FlowNodeInfo>.Fail("节点类型不能为null");
            }



            var nodeInfo = new FlowNodeInfo
            {
                Id = Utils.GenId(),
                InputFields = new List<InputField>(),
                OutputFields = new List<OutputField>(),
                Signals = new List<SignalInfo>(),
                Emits = new List<EmitInfo>(),
                NodeImplType = nodeType,
                NodeImplTypeFullName = nodeType.FullName,
            };

            // 判断节点类型
            if (IsFlowExpression(nodeType))
            {
                nodeInfo.Kind = NodeKind.Expression;
                // 获取表达式名称
                var flowExpressionAttr = nodeType.GetCustomAttribute<FlowExprAttribute>();
                if (flowExpressionAttr != null && !string.IsNullOrEmpty(flowExpressionAttr.Label))
                {
                    nodeInfo.Label = new Label(nodeType.Name, flowExpressionAttr.Label);
                }
                else
                {
                    nodeInfo.Label = new Label(nodeType.Name, nodeType.Name);
                }
            }
            else
            {
                // 必须继承基类
                if (!typeof(FlowBaseNode).IsAssignableFrom(nodeType))
                {
                    return Res<FlowNodeInfo>.Fail($"类型{nodeType.Name}必须继承FlowBaseNode");
                }

                nodeInfo.Kind = NodeKind.Node;
                if (nodeType == typeof(StartNode))
                {
                    nodeInfo.Kind = NodeKind.StartNode;
                }
                // 获取节点名称
                var flowNodeAttr = nodeType.GetCustomAttribute<FlowNodeAttribute>();
                nodeInfo.Label = new Label(nodeType.Name, flowNodeAttr?.Label ?? nodeType.Name);
            }

            // 获取所有属性
            var properties = nodeType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // 处理输入字段
            foreach (var prop in properties)
            {

                var inputAttr = prop.GetCustomAttribute<FlowInputAttribute>();
                if (inputAttr != null)
                {
                    var inputField = new InputField
                    {
                        Label = new Label(prop.Name, inputAttr.Label),
                        Type = prop.PropertyType.Name,
                        Required = inputAttr.Required,
                        PropertyInfo = prop
                    };

                    nodeInfo.InputFields.Add(inputField);
                }

                var outputAttr = prop.GetCustomAttribute<FlowOutputAttribute>();
                if (outputAttr != null)
                {
                    var outputField = new OutputField
                    {
                        Label = new Label(prop.Name, outputAttr.Label),
                        Type = prop.PropertyType.Name,
                        PropertyInfo = prop
                    };

                    nodeInfo.OutputFields.Add(outputField);
                }

                // 同一个prop不能同时作为输入和输出
                if(nodeInfo.InputFields.Any(f=>f.PropertyInfo == prop) && nodeInfo.OutputFields.Any(f=>f.PropertyInfo == prop))
                {
                    return Res<FlowNodeInfo>.Fail("同一个属性不能同时作为输入和输出");
                }
            
            }

            // 处理信号（方法）
            var methods = nodeType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var method in methods)
            {
                var signalAttr = method.GetCustomAttribute<FlowSignalAttribute>();
                if (signalAttr != null)
                {
                    var signalInfo = new SignalInfo
                    {
                        Label = new Label(method.Name, signalAttr.Label),
                        MethodInfo = method
                    };

                    // 判断方法是否是以FlowOutEvent类型或void类型返回
                    if(method.ReturnType != typeof(FlowOutEvent) && method.ReturnType != typeof(void))
                    {
                        return Res<FlowNodeInfo>.Fail($"方法{method.Name}的返回类型不能是{method.ReturnType.Name},必须是FlowOutEvent类型或void类型");
                    }

                    nodeInfo.Signals.Add(signalInfo);
                }
            }

            // 处理输出事件
            var events = nodeType.GetEvents(BindingFlags.Public | BindingFlags.Instance);
            foreach (var evt in events)
            {
                var emitAttr = evt.GetCustomAttribute<FlowEventAttribute>();
                if (emitAttr != null)
                {
                    var emitInfo = new EmitInfo
                    {
                        Label = new Label(evt.Name, emitAttr.Label),
                        EventInfo = evt
                    };
                    nodeInfo.Emits.Add(emitInfo);
                }
            }

            return Res<FlowNodeInfo>.Ok(nodeInfo);
        }

        /// <summary>
        /// 判断是否为FlowExpression类型
        /// </summary>
        private static bool IsFlowExpression(Type type)
        {
            if (type == null) return false;
            
            // 检查基类型是否为FlowExpression<T>
            var baseType = type.BaseType;
            while (baseType != null)
            {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(JstFlow.Core.Metas.FlowExpression<>))
                {
                    return true;
                }
                baseType = baseType.BaseType;
            }
            
            return false;
        }

    }
}
