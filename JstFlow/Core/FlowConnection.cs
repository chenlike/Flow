using JstFlow.Core.NodeMeta;
using System;
using System.Collections.Generic;
using System.Text;
using JstFlow.Common;
using System.Linq.Expressions;
using JstFlow.External.Nodes;

namespace JstFlow.Core
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
            long sourceNodeId,
            long targetNodeId, 
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
            var targetMemberName = GetMemberName(targetExpression);
            if (string.IsNullOrEmpty(targetMemberName))
            {
                throw new ArgumentException("无效的目标表达式，无法提取信号名称");
            }

            return new FlowConnection
            {
                Type = ConnectionType.EventToSignal,
                SourceNodeId = sourceNodeId,
                TargetNodeId = targetNodeId,
                SourceEndpointCode = sourceMemberName,
                TargetEndpointCode = targetMemberName
            };
        }

        /// <summary>
        /// 从表达式中提取成员名称
        /// </summary>
        private static string GetMemberName<T>(Expression<Func<T, object>> expression)
        {
            if (expression is LambdaExpression lambda &&
                lambda.Body is MemberExpression memberExpr)
            {
                return memberExpr.Member.Name;
            }
            return null;
        }

        /// <summary>
        /// 从函数表达式中提取成员名称
        /// </summary>
        private static string GetMemberName<T>(Expression<Func<T, Func<object>>> expression)
        {
            if (expression is LambdaExpression lambda &&
                lambda.Body is MemberExpression memberExpr)
            {
                return memberExpr.Member.Name;
            }
            return null;
        }


    }
}