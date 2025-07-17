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
using System.Threading.Tasks;


namespace JstFlow.Core
{
    public class FlowNodeBuilder
    {
        public static Res<FlowNodeInfo> Build(Type nodeType)
        {
            if (nodeType == null)
                return Res<FlowNodeInfo>.Fail("节点类型不能为null");

            var nodeInfo = new FlowNodeInfo
            {
                Id = Utils.GenId(),
                InputFields = new List<InputField>(),
                OutputFields = new List<OutputField>(),
                Signals = new List<SignalInfo>(),
                Emits = new List<EmitInfo>(),
                NodeImplType = nodeType,
                NodeImplTypeFullName = nodeType.FullName
            };

            var result = SetNodeKindAndLabel(nodeType, nodeInfo);
            if (!result.IsSuccess) return result;

            var props = nodeType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var propResult = ProcessProperties(props, nodeInfo);
            if (!propResult.IsSuccess) return propResult;

            var methods = nodeType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var signalResult = ProcessSignals(methods, nodeInfo);
            if (!signalResult.IsSuccess) return signalResult;

            var events = nodeType.GetEvents(BindingFlags.Public | BindingFlags.Instance);
            ProcessEmits(events, nodeInfo);

            return Res<FlowNodeInfo>.Ok(nodeInfo);
        }

        private static Res<FlowNodeInfo> SetNodeKindAndLabel(Type nodeType, FlowNodeInfo nodeInfo)
        {
            if (IsFlowExpression(nodeType))
            {
                nodeInfo.Kind = NodeKind.Expression;
                var attr = nodeType.GetCustomAttribute<FlowExprAttribute>();
                nodeInfo.Label = CreateLabel(nodeType.Name, attr?.Label);
            }
            else
            {
                if (!typeof(FlowBaseNode).IsAssignableFrom(nodeType))
                    return Res<FlowNodeInfo>.Fail($"类型 {nodeType.Name} 必须继承 FlowBaseNode");

                nodeInfo.Kind = nodeType == typeof(StartNode) ? NodeKind.StartNode : NodeKind.Node;
                var attr = nodeType.GetCustomAttribute<FlowNodeAttribute>();
                nodeInfo.Label = CreateLabel(nodeType.Name, attr?.Label);
            }

            return Res<FlowNodeInfo>.Ok(nodeInfo);
        }

        private static Res<FlowNodeInfo> ProcessProperties(PropertyInfo[] properties, FlowNodeInfo nodeInfo)
        {
            foreach (var prop in properties)
            {
                var inputAttr = prop.GetCustomAttribute<FlowInputAttribute>();
                if (inputAttr != null)
                {
                    nodeInfo.InputFields.Add(new InputField
                    {
                        Label = CreateLabel(prop.Name, inputAttr.Label),
                        Type = prop.PropertyType.Name,
                        Required = inputAttr.Required,
                        PropertyInfo = prop
                    });
                }

                var outputAttr = prop.GetCustomAttribute<FlowOutputAttribute>();
                if (outputAttr != null)
                {
                    nodeInfo.OutputFields.Add(new OutputField
                    {
                        Label = CreateLabel(prop.Name, outputAttr.Label),
                        Type = prop.PropertyType.Name,
                        PropertyInfo = prop
                    });
                }

                if (nodeInfo.InputFields.Any(f => f.PropertyInfo == prop) &&
                    nodeInfo.OutputFields.Any(f => f.PropertyInfo == prop))
                {
                    return Res<FlowNodeInfo>.Fail($"属性 {prop.Name} 不能同时作为输入和输出");
                }
            }

            return Res<FlowNodeInfo>.Ok(nodeInfo);
        }

        private static Res<FlowNodeInfo> ProcessSignals(MethodInfo[] methods, FlowNodeInfo nodeInfo)
        {
            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<FlowSignalAttribute>();
                if (attr == null) continue;

                if (!IsValidSignalReturnType(method.ReturnType))
                {
                    return Res<FlowNodeInfo>.Fail(
                        $"方法 {method.Name} 的返回类型 {method.ReturnType.Name} 非法，必须为 void 或 FlowOutEvent ,IEnumable<FlowOutEvent>等有效类型");
                }

                nodeInfo.Signals.Add(new SignalInfo
                {
                    Label = CreateLabel(method.Name, attr.Label),
                    MethodInfo = method
                });
            }

            return Res<FlowNodeInfo>.Ok(nodeInfo);
        }

        private static void ProcessEmits(EventInfo[] events, FlowNodeInfo nodeInfo)
        {
            foreach (var evt in events)
            {
                var attr = evt.GetCustomAttribute<FlowEventAttribute>();
                if (attr != null)
                {
                    nodeInfo.Emits.Add(new EmitInfo
                    {
                        Label = CreateLabel(evt.Name, attr.Label),
                        EventInfo = evt
                    });
                }
            }
        }

        private static bool IsFlowExpression(Type type)
        {
            while (type?.BaseType != null)
            {
                if (type.BaseType.IsGenericType &&
                    type.BaseType.GetGenericTypeDefinition() == typeof(FlowExpression<>))
                    return true;

                type = type.BaseType;
            }
            return false;
        }

        private static bool IsValidSignalReturnType(Type returnType)
        {
            return returnType == typeof(void) ||
                   returnType == typeof(FlowOutEvent) ||
                   returnType == typeof(IEnumerator<FlowOutEvent>) ||
                   returnType == typeof(IEnumerable<FlowOutEvent>);
        }

        private static Label CreateLabel(string name, string displayName)
        {
            return new Label(name, string.IsNullOrWhiteSpace(displayName) ? name : displayName);
        }
    }

}
