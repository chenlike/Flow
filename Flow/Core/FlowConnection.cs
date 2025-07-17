using Flow.Core.NodeMeta;
using System;
using System.Collections.Generic;
using System.Text;
using Flow.Common;
using System.Linq.Expressions;
using Flow.External.Nodes;
using System.Linq;
using Flow.Core.Metas;
using System.Reflection;

namespace Flow.Core
{
    /// <summary>
    /// 连接类型
    /// </summary>
    public enum ConnectionType
    {
        /// <summary>
        /// 输出字段连接到输入字段
        /// </summary>
        OutputToInput,

        /// <summary>
        /// 事件连接到信号
        /// </summary>
        EventToSignal,
    }

    /// <summary>
    /// 表示节点之间的连接
    /// </summary>
    public class FlowConnection
    {
        /// <summary>
        /// 连接的唯一标识
        /// </summary>
        public long Id { get; set; } = Utils.GenId();

        /// <summary>
        /// 连接类型
        /// </summary>
        public ConnectionType Type { get; set; }

        /// <summary>
        /// 源节点id
        /// </summary>
        public long SourceNodeId { get; set; }

        /// <summary>
        /// 目标节点id
        /// </summary>
        public long TargetNodeId { get; set; }

        /// <summary>
        /// 源端点名称（输出字段名或事件名）
        /// </summary>
        public string SourceEndpointCode { get; set; }

        /// <summary>
        /// 目标端点名称（输入字段名或信号名）
        /// </summary>
        public string TargetEndpointCode { get; set; }



        public static FlowConnection EventToSignal<TSourceNode, TTargetNode>(
            FlowNodeInfo sourceNodeInfo,
            FlowNodeInfo targetNodeInfo, 
            Expression<Func<TSourceNode, object>> sourceExpression,
            Expression<Func<TTargetNode, Func<object>>> targetExpression
        ) where TSourceNode : FlowBaseNode
        where TTargetNode : FlowBaseNode
        {
            // 从源表达式中提取事件名称
            var sourceMemberName = GetMemberName(sourceExpression);
            if (string.IsNullOrEmpty(sourceMemberName))
            {
                throw new ArgumentException("无效的源表达式，无法提取事件名称");
            }

            // 从目标表达式中提取信号名称
            var targetMemberName = ExtractMethodName(targetExpression.Body);
            if (string.IsNullOrEmpty(targetMemberName))
            {
                throw new ArgumentException("无效的目标表达式，无法提取信号名称");
            }

            // 验证源节点的事件是否存在
            var sourceEvent = sourceNodeInfo.Emits?.FirstOrDefault(e => e.PropertyInfo.Name == sourceMemberName);
            if (sourceEvent == null)
            {
                throw new ArgumentException($"源节点中不存在事件 '{sourceMemberName}'");
            }

            // 验证目标节点的信号是否存在
            var targetSignal = targetNodeInfo.Signals?.FirstOrDefault(s => s.MethodInfo.Name == targetMemberName);
            if (targetSignal == null)
            {
                throw new ArgumentException($"目标节点中不存在信号 '{targetMemberName}'");
            }

            return new FlowConnection
            {
                Type = ConnectionType.EventToSignal,
                SourceNodeId = sourceNodeInfo.Id,
                TargetNodeId = targetNodeInfo.Id,
                SourceEndpointCode = sourceMemberName,
                TargetEndpointCode = targetMemberName
            };
        }


        public static FlowConnection OutputToInput<TSourceNode, TTargetNode>(
            FlowNodeInfo sourceNodeInfo,
            FlowNodeInfo targetNodeInfo, 
            Expression<Func<TSourceNode, object>> sourceExpression,
            Expression<Func<TTargetNode, object>> targetExpression
        ) where TSourceNode : FlowBaseNode
        where TTargetNode : FlowBaseNode
        {
            // 从源表达式中提取输出字段名称
            var sourceMemberName = GetMemberName(sourceExpression);
            if (string.IsNullOrEmpty(sourceMemberName))
            {
                throw new ArgumentException("无效的源表达式，无法提取输出字段名称");
            }

            // 从目标表达式中提取输入字段名称
            var targetMemberName = GetMemberName(targetExpression);
            if (string.IsNullOrEmpty(targetMemberName))
            {
                throw new ArgumentException("无效的目标表达式，无法提取输入字段名称");
            }

            // 验证源节点的输出字段是否存在
            var sourceOutputField = sourceNodeInfo.OutputFields?.FirstOrDefault(f => f.PropertyInfo.Name == sourceMemberName);
            if (sourceOutputField == null)
            {
                throw new ArgumentException($"源节点中不存在输出字段 '{sourceMemberName}'");
            }

            // 验证目标节点的输入字段是否存在
            var targetInputField = targetNodeInfo.InputFields?.FirstOrDefault(f => f.PropertyInfo.Name == targetMemberName);
            if (targetInputField == null)
            {
                throw new ArgumentException($"目标节点中不存在输入字段 '{targetMemberName}'");
            }

            return new FlowConnection
            {
                Type = ConnectionType.OutputToInput,
                SourceNodeId = sourceNodeInfo.Id,
                TargetNodeId = targetNodeInfo.Id,
                SourceEndpointCode = sourceMemberName,
                TargetEndpointCode = targetMemberName
            };
        }


        /// <summary>
        /// 从表达式中提取成员名称
        /// </summary>
        private static string GetMemberName<T, TResult>(Expression<Func<T, TResult>> expression)
        {
            if (expression is LambdaExpression lambda &&
                lambda.Body is MemberExpression memberExpr)
            {
                return memberExpr.Member.Name;
            }
            return null;
        }

        /// <summary>
        /// 从表达式中提取方法名称
        /// </summary>
        private static string ExtractMethodName(Expression expr)
        {
            // 递归剥离 Convert、Quote 等包装
            while (expr is UnaryExpression unary &&
                   (unary.NodeType == ExpressionType.Convert || unary.NodeType == ExpressionType.Quote))
            {
                expr = unary.Operand;
            }

            // 情况 1：t => t.Print 解析成 MemberExpression
            if (expr is MemberExpression memberExpr)
            {
                return memberExpr.Member.Name;
            }

            // 情况 2：MethodCallExpression，如 MethodInfo.CreateDelegate(...)
            if (expr is MethodCallExpression methodCall)
            {
                // 检查参数中是否包含 Constant(MethodInfo)
                foreach (var arg in methodCall.Arguments)
                {
                    if (arg is ConstantExpression ce && ce.Value is MethodInfo mi)
                    {
                        return mi.Name;
                    }
                }

                // 有些表达式把 MethodInfo 放在 Object 上
                if (methodCall.Object is ConstantExpression objConst && objConst.Value is MethodInfo objMethod)
                {
                    return objMethod.Name;
                }
            }

            return null;
        }


    }
}