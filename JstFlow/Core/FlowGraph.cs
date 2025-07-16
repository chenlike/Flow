using JstFlow.Core.NodeMeta;
using JstFlow.Core.Metas;
using System;
using System.Collections.Generic;
using System.Text;
using JstFlow.Common;
using System.Linq;
using JstFlow.External;

namespace JstFlow.Core
{
    /// <summary>
    /// 流程图，包含节点和连接
    /// </summary>
    public partial class FlowGraph
    {
        /// <summary>
        /// 图的唯一标识
        /// </summary>
        public long Id { get; set; } = Utils.GenId();

        /// <summary>
        /// 节点
        /// </summary>
        public List<FlowNodeInfo> Nodes { get; set; } = new List<FlowNodeInfo>();

        /// <summary>
        /// 连接
        /// </summary>
        public List<FlowConnection> Connections { get; set; } = new List<FlowConnection>();



        

        private FlowGraph(){}


        public static Res<FlowGraph> Create(List<FlowNodeInfo> nodes, List<FlowConnection> connections)
        {
            var graph = new FlowGraph();
            graph.Nodes = nodes;
            graph.Connections = connections;
        
            var validateRes = graph.ValidateGraph();
            if (validateRes.IsFailure)
            {
                return Res<FlowGraph>.Fail(validateRes.Message);
            }
            return Res<FlowGraph>.Ok(graph, validateRes.Message);
        }



    


    }


}
