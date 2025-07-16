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

            var node = _nodeMap[CurrentNodeId];


            // 检查输入 根据输入的连线找获取对应的值,如果没有则认为是默认值
            // 如果输入是来自于expression 需要先执行获得结果


            // 监听event

            // 执行操作


            
            

        }

    }


}
