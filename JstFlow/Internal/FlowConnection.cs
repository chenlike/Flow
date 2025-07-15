using JstFlow.Internal.NodeMeta;
using System;
using System.Collections.Generic;
using System.Text;
using JstFlow.Common;

namespace JstFlow.Internal
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

        /// <summary>
        /// 获取连接的描述信息
        /// </summary>
        public string GetDescription()
        {
            switch (Type)
            {
                case ConnectionType.OutputToInput:
                    return $"输出字段 '{SourceEndpointCode}' -> 输入字段 '{TargetEndpointCode}'";
                case ConnectionType.EventToSignal:
                    return $"事件 '{SourceEndpointCode}' -> 信号 '{TargetEndpointCode}'";
                default:
                    return $"未知连接类型: {Type}";
            }
        }

        /// <summary>
        /// 获取完整的连接描述（包含节点信息）
        /// </summary>
        public string GetFullDescription()
        {
            return $"节点 {SourceNodeId} 的 {GetDescription()} -> 节点 {TargetNodeId}";
        }
    }
} 