using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using JstFlow.Internal.NodeMeta;
using JstFlow.Internal.Metas;

namespace JstFlow.Internal.Utils
{
    /// <summary>
    /// FlowGraph 的 JSON 序列化/反序列化工具
    /// </summary>
    public static class FlowGraphJsonUtil
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        /// <summary>
        /// 序列化 FlowGraph 到 JSON 字符串
        /// </summary>
        public static string Serialize(FlowGraph graph)
        {
            return JsonSerializer.Serialize(graph, _options);
        }

        /// <summary>
        /// 从 JSON 字符串反序列化为 FlowGraph
        /// </summary>
        public static FlowGraph Deserialize(string json)
        {
            return JsonSerializer.Deserialize<FlowGraph>(json, _options);
        }
    }
} 