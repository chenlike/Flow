using JstFlow.External;
using JstFlow.Core;
using JstFlow.Attributes;
using JstFlow.External.Nodes;
using Xunit;
using System;
using System.Reflection;

namespace Test.JstFlow.Node
{
    public class StartNodeTests
    {
        [Fact]
        public void StartNode_ShouldHaveCorrectAttributes()
        {
            // Arrange & Act
            var startNode = new StartNode();
            
            // Assert
            var nodeAttribute = typeof(StartNode).GetCustomAttributes(typeof(FlowNodeAttribute), false);
            Assert.Single(nodeAttribute);
            Assert.Equal("开始", ((FlowNodeAttribute)nodeAttribute[0]).Label);
        }

        [Fact]
        public void StartNode_ShouldHaveStartEvent()
        {
            // Arrange
            var startNode = new StartNode();
            
            // Act & Assert
            // Start 属性应该存在，但可能为 null
            Assert.True(typeof(StartNode).GetProperty("Start") != null);
        }

        [Fact]
        public void StartNode_StartLoop_ShouldReturnFlowOutEvent()
        {
            // Arrange
            var startNode = new StartNode();
            var context = new NodeContext();
            startNode.Inject(context);
            
            // Act
            var result = startNode.StartLoop();
            
            // Assert
            Assert.NotNull(result);
            Assert.IsType<FlowOutEvent>(result);
        }

        [Fact]
        public void StartNode_StartLoop_ShouldReturnCorrectExpression()
        {
            // Arrange
            var startNode = new StartNode();
            var context = new NodeContext();
            startNode.Inject(context);
            
            // Act
            var result = startNode.StartLoop();
            
            // Assert
            Assert.NotNull(result.Expression);
            var compiledExpression = result.Expression.Compile();
            Assert.Equal(startNode.Start, compiledExpression());
        }

        [Fact]
        public void StartNode_ShouldHaveCorrectSignalAttribute()
        {
            // Arrange
            var startNode = new StartNode();
            
            // Act
            var methodInfo = typeof(StartNode).GetMethod("StartLoop");
            var signalAttribute = methodInfo.GetCustomAttributes(typeof(FlowSignalAttribute), false);
            
            // Assert
            Assert.Single(signalAttribute);
            Assert.Equal("开始循环", ((FlowSignalAttribute)signalAttribute[0]).Label);
        }
    }
} 