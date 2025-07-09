using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;
using JstFlow.Internal.NodeMeta;
using JstFlow.Attributes;
using JstFlow.Common;
using JstFlow.Internal.Metas;

namespace JstFlow.Internal
{
    public class NodeFactory
    {
        public static FlowNodeMeta CreateNodeInfo(Type nodeType)
        {
            var nodeInfo = new FlowNodeMeta
            {
                InputFields = new List<InputField>(),
                OutputFields = new List<OutputField>(),
                Signals = new List<SignalInfo>(),
                Emits = new List<EmitInfo>(),
                NodeImplType = nodeType,
            };

            // 判断节点类型
            if (IsFlowExpression(nodeType))
            {
                nodeInfo.Kind = NodeKind.Expression;
                // 获取表达式名称
                var flowExpressionAttr = nodeType.GetCustomAttribute<FlowExpressionAttribute>();
                if (flowExpressionAttr != null && !string.IsNullOrEmpty(flowExpressionAttr.Label))
                {
                    nodeInfo.Name = new Label(nodeType.Name, flowExpressionAttr.Label);
                }
                else
                {
                    nodeInfo.Name = new Label(nodeType.Name, nodeType.Name);
                }
            }
            else
            {
                nodeInfo.Kind = NodeKind.Node;
                // 获取节点名称
                var flowNodeAttr = nodeType.GetCustomAttribute<FlowNodeAttribute>();
                nodeInfo.Name = new Label(nodeType.Name, flowNodeAttr?.Label ?? nodeType.Name);
            }

            // 获取所有属性
            var properties = nodeType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // 处理输入字段
            foreach (var prop in properties)
            {
                var inputAttr = prop.GetCustomAttribute<InputAttribute>();
                if (inputAttr != null)
                {
                    var inputField = new InputField
                    {
                        Name = new Label(prop.Name, inputAttr.Label),
                        Type = prop.PropertyType.Name,
                        Required = inputAttr.Required,
                        PropertyInfo = prop
                    };
                    nodeInfo.InputFields.Add(inputField);
                }

                var outputAttr = prop.GetCustomAttribute<OutputAttribute>();
                if (outputAttr != null)
                {
                    var outputField = new OutputField
                    {
                        Name = new Label(prop.Name, outputAttr.Label),
                        Type = prop.PropertyType.Name,
                        PropertyInfo = prop
                    };
                    nodeInfo.OutputFields.Add(outputField);
                }
            }

            // 处理信号（方法）
            var methods = nodeType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var method in methods)
            {
                var signalAttr = method.GetCustomAttribute<SignalAttribute>();
                if (signalAttr != null)
                {
                    var signalInfo = new SignalInfo
                    {
                        Name = new Label(method.Name, signalAttr.Label),
                        MethodInfo = method
                    };
                    nodeInfo.Signals.Add(signalInfo);
                }
            }

            // 处理输出事件
            var events = nodeType.GetEvents(BindingFlags.Public | BindingFlags.Instance);
            foreach (var evt in events)
            {
                var emitAttr = evt.GetCustomAttribute<EmitAttribute>();
                if (emitAttr != null)
                {
                    var emitInfo = new EmitInfo
                    {
                        Name = new Label(evt.Name, emitAttr.Label),
                        EventInfo = evt
                    };
                    nodeInfo.Emits.Add(emitInfo);
                }
            }

            return nodeInfo;
        }

        /// <summary>
        /// 判断是否为FlowExpression类型
        /// </summary>
        private static bool IsFlowExpression(Type type)
        {
            if (type == null) return false;
            
            // 检查基类型是否为FlowExpression
            var baseType = type.BaseType;
            while (baseType != null)
            {
                if (baseType == typeof(FlowExpression))
                {
                    return true;
                }
                baseType = baseType.BaseType;
            }
            
            return false;
        }

        /// <summary>
        /// 获取表达式标签
        /// </summary>
        private static string GetExpressionLabel(Type type)
        {
            // 尝试获取FlowExpressionAttribute
            var flowExpressionAttr = type.GetCustomAttribute<FlowExpressionAttribute>();
            if (flowExpressionAttr != null && !string.IsNullOrEmpty(flowExpressionAttr.Label))
            {
                return flowExpressionAttr.Label;
            }
            
            // 如果没有属性或标签为空，返回类型名称
            return type.Name;
        }
    }
}
