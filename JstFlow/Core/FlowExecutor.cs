using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;
using JstFlow.Core.NodeMeta;
using JstFlow.Core.Metas;
using System.Threading.Tasks;
using JstFlow.External;
using JstFlow.Common;

namespace JstFlow.Core
{
    /// <summary>
    /// 流程图执行器，负责执行FlowGraph中定义的节点和连接
    /// </summary>
    public class FlowExecutor
    {
        /// <summary>
        /// 要执行的流程图
        /// </summary>
        private FlowGraph _flowGraph;
 

        private Dictionary<long, FlowNodeInfo> _nodeMap;



        public long CurrentNodeId { get; private set; }



        
 


        private FlowExecutor(){}

        public static Res<FlowExecutor> Create(FlowGraph flowGraph){
            var executor = new FlowExecutor();
            executor._flowGraph = flowGraph;
            var validateRes = flowGraph.ValidateGraph();
            if (validateRes.IsFailure)
            {
                return Res<FlowExecutor>.Fail(validateRes.Message);
            }
            executor._nodeMap = flowGraph.Nodes.ToDictionary(node => node.Id, node => node);
            return Res<FlowExecutor>.Ok(executor, validateRes.Message);
        }

        public void Start(){
            CurrentNodeId = _flowGraph.Nodes.Where(node => node.Kind == NodeKind.StartNode).First().Id;
        }

        public void Execute()
        {



        

        }



        private void PrepareNode(long nodeId)
        {
            var node = _nodeMap[nodeId];

            var nodeConnections = _flowGraph.Connections.Where(connection => connection.TargetNodeId == nodeId).ToList();

            Dictionary<string, object> inputValues = new Dictionary<string, object>();

            // 获得这个节点input参数的连线 
            foreach (InputField input in node.InputFields)
            {

                // 查找connection中作为字段连接的
                var fieldConnection = nodeConnections.FirstOrDefault(connection => connection.Type == ConnectionType.OutputToInput && connection.TargetEndpointCode == input.Label.Code);
                if (fieldConnection == null)
                {
                    // 这个字段没有连线
                    continue;
                }



                // 获得这个字段连接的sourceNodeId
                var sourceNodeId = fieldConnection.SourceNodeId;

                // 获得这个sourceNodeId的节点
                var sourceNode = _nodeMap[sourceNodeId];

                // 根据Code 获得这个sourceNode的outputField  
                var outputField = sourceNode.OutputFields.FirstOrDefault(field => field.Label.Code == fieldConnection.SourceEndpointCode);

                

                // 判断如果是来自expression的则直接执行获得值




            }

        }




    }


}
