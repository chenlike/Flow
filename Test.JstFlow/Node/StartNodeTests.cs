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
        public void StartNode_Execute_ShouldReturnFlowOutEvent()
        {
            // Arrange
            var startNode = new StartNode();
            
            // Act
            var result = startNode.Execute();
            
            // Assert
            Assert.NotNull(result);
            Assert.IsType<FlowOutEvent>(result);
        }

        [Fact]
        public void StartNode_Execute_ShouldReturnCorrectMemberName()
        {
            // Arrange
            var startNode = new StartNode();
            
            // Act
            var result = startNode.Execute();
            
            // Assert
            Assert.Equal("Start", result.MemberName);
        }

        [Fact]
        public void StartNode_ShouldHaveCorrectSignalAttribute()
        {
            // Arrange
            var startNode = new StartNode();
            
            // Act
            var methodInfo = typeof(StartNode).GetMethod("Execute");
            var signalAttribute = methodInfo.GetCustomAttributes(typeof(FlowSignalAttribute), false);
            
            // Assert
            Assert.Single(signalAttribute);
            Assert.Equal("开始循环", ((FlowSignalAttribute)signalAttribute[0]).Label);
        }
    }
} 