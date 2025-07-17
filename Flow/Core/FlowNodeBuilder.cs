using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;
using Flow.Core.NodeMeta;
using Flow.Attributes;
using Flow.Common;
using Flow.Core.Metas;
using Flow.External;
using Flow.External.Nodes;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Flow.Core
{
    public class FlowNodeBuilderChain<T> where T : FlowBaseNode
    {
        private readonly Type _nodeType;
        private readonly Dictionary<string, object> _initValues;

        internal FlowNodeBuilderChain(Type nodeType)
        {
            _nodeType = nodeType;
            _initValues = new Dictionary<string, object>();
        }

        public FlowNodeBuilderChain<T> SetValue<TProperty>(Expression<Func<T, TProperty>> propertySelector, TProperty value)
        {
            var memberExpression = propertySelector.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException("表达式必须是属性访问表达式");
            }

            var propertyName = memberExpression.Member.Name;
            _initValues[propertyName] = value;
            return this;
        }

        public FlowNodeInfo Build()
        {
            var res = FlowNodeBuilder.Build(_nodeType, _initValues);
            if (res.IsFailure)
            {
                throw new Exception(res.Message);
            }
            return res.Data;
        }
    }

    public class FlowNodeBuilder
    {
        /// <summary>
        /// 链式构建节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static FlowNodeBuilderChain<T> For<T>() where T : FlowBaseNode
        {
            return new FlowNodeBuilderChain<T>(typeof(T));
        }

        public static FlowNodeInfo Build<TType>() where TType:FlowBaseNode{
            var res = Build(typeof(TType));
            if(res.IsFailure)
            {
                throw new Exception(res.Message);
            }
            return res.Data;
        }
        public static Res<FlowNodeInfo> Build(Type nodeType,Dictionary<string,object> initValues = null)
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
            var propResult = ProcessProperties(props, nodeInfo,initValues);
            if (!propResult.IsSuccess) return propResult;

            var methods = nodeType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var signalResult = ProcessSignals(methods, nodeInfo);
            if (!signalResult.IsSuccess) return signalResult;

            // 处理属性类型的emits
            ProcessPropertyEmits(props, nodeInfo);

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

        private static Res<FlowNodeInfo> ProcessProperties(PropertyInfo[] properties, FlowNodeInfo nodeInfo,Dictionary<string,object> initValues = null)
        {
            foreach (var prop in properties)
            {
                var inputAttr = prop.GetCustomAttribute<FlowInputAttribute>();
                if (inputAttr != null)
                {
                    var field = new InputField
                    {
                        Label = CreateLabel(prop.Name, inputAttr.Label),
                        Type = prop.PropertyType.Name,
                        Required = inputAttr.Required,
                        PropertyInfo = prop,
                    };
                    
                    // 传入的初始值优先级最高
                    if(initValues != null && initValues.ContainsKey(prop.Name))
                    {
                        field.InitValue = initValues[prop.Name];
                    }
                    else
                    {
                        // 如果没有传入初始值，尝试从属性本身获取初始值
                        try
                        {
                            var instance = Activator.CreateInstance(nodeInfo.NodeImplType);
                            var value = prop.GetValue(instance);
                            if (value != null && !value.Equals(GetDefaultValue(prop.PropertyType)))
                            {
                                field.InitValue = value;
                            }
                        }
                        catch
                        {
                            // 如果无法创建实例或获取值失败，忽略错误
                        }
                    }
                    
                    nodeInfo.InputFields.Add(field);
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

        private static object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
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



        private static void ProcessPropertyEmits(PropertyInfo[] properties, FlowNodeInfo nodeInfo)
        {
            // 处理属性类型的emits（FlowEndpoint类型）
            foreach (var prop in properties)
            {
                var attr = prop.GetCustomAttribute<FlowEventAttribute>();
                if (attr != null && prop.PropertyType == typeof(FlowEndpoint))
                {
                    nodeInfo.Emits.Add(new EmitInfo
                    {
                        Label = CreateLabel(prop.Name, attr.Label),
                        PropertyInfo = prop
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
