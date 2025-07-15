using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;
using JstFlow.Internal.NodeMeta;
using JstFlow.Attributes;
using JstFlow.Common;
using JstFlow.Internal.Metas;
using JstFlow.External;


namespace JstFlow.Internal
{
    public class NodeFactory
    {
        public static FlowNode CreateNodeInfo(Type nodeType)
        {
            var nodeInfo = new FlowNode
            {
                Id = Utils.GenId(),
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
                var inputAttr = prop.GetCustomAttribute<InputAttribute>();
                if (inputAttr != null)
                {
                    var inputField = new InputField
                    {
                        Label = new Label(prop.Name, inputAttr.Label),
                        Type = prop.PropertyType.Name,
                        Required = inputAttr.Required,
                        PropertyInfo = prop
                    };

                    // 检查是否为泛型参数
                    if (prop.PropertyType.IsGenericParameter)
                    {
                        inputField.IsGenericParameter = true;
                        inputField.GenericParameterName = prop.PropertyType.Name;
                        
                        // 获取泛型约束
                        var constraints = prop.PropertyType.GetGenericParameterConstraints();
                        foreach (var constraint in constraints)
                        {
                            inputField.GenericConstraints.Add(constraint.Name);
                        }
                    }

                    nodeInfo.InputFields.Add(inputField);
                }

                var outputAttr = prop.GetCustomAttribute<OutputAttribute>();
                if (outputAttr != null)
                {
                    var outputField = new OutputField
                    {
                        Label = new Label(prop.Name, outputAttr.Label),
                        Type = prop.PropertyType.Name,
                        PropertyInfo = prop
                    };

                    // 检查是否为泛型参数
                    if (prop.PropertyType.IsGenericParameter)
                    {
                        outputField.IsGenericParameter = true;
                        outputField.GenericParameterName = prop.PropertyType.Name;
                        
                        // 获取泛型约束
                        var constraints = prop.PropertyType.GetGenericParameterConstraints();
                        foreach (var constraint in constraints)
                        {
                            outputField.GenericConstraints.Add(constraint.Name);
                        }
                    }

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
                        Label = new Label(method.Name, signalAttr.Label),
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
                        Label = new Label(evt.Name, emitAttr.Label),
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
            
            // 检查基类型是否为FlowExpression<T>
            var baseType = type.BaseType;
            while (baseType != null)
            {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(FlowExpression<>))
                {
                    return true;
                }
                baseType = baseType.BaseType;
            }
            
            return false;
        }

    }
}
