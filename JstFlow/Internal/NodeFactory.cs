using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;
using JstFlow.Internal.NodeMeta;
using JstFlow.Attributes;
using JstFlow.Common;

namespace JstFlow.Internal
{
    public class NodeFactory
    {
        public static NodeInfo CreateNodeInfo(Type nodeType)
        {
            var nodeInfo = new NodeInfo
            {
                InputFields = new List<InputField>(),
                OutputFields = new List<OutputField>(),
                Signals = new List<SignalInfo>(),
                Emits = new List<EmitInfo>(),
                NodeType = nodeType,

            };

            // 获取节点名称
            var flowNodeAttr = nodeType.GetCustomAttribute<FlowNodeAttribute>();
            nodeInfo.Name = new Label(nodeType.Name, flowNodeAttr?.Label ?? nodeType.Name);

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
    }
}
