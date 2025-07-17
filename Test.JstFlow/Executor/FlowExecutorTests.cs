using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JstFlow.Core;
using JstFlow.Core.NodeMeta;
using JstFlow.External;
using JstFlow.External.Expressions;
using JstFlow.External.Nodes;
using Xunit;

namespace Test.JstFlow.Executor
{
    public class FlowExecutorTests
    {

        [Fact]
        public void Test()
        {

            FlowNodeInfo forNode = FlowNodeBuilder.Build<ForNode>();



            FlowNodeInfo debugNode = FlowNodeBuilder.Build<DebugLogNode>();

            var variables = new Dictionary<string, VariableExpression<int>>()
            {
                { "Start", new VariableExpression<int>(0) },
                { "End", new VariableExpression<int>(10) },
            };

            var connections = new List<FlowConnection>()
            {
                FlowConnection.EventToSignal<ForNode, DebugLogNode>(forNode.Id, debugNode.Id, n => n.LoopBody, n => n.Print)
            };







        }






    }
}
