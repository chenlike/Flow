using JstFlow.External.Nodes;
using JstFlow.Core;
using JstFlow.Attributes;
using Xunit;
using System;
using System.Linq.Expressions;

namespace Test.JstFlow.Node
{
    public class FlowBaseNodeTests
    {
        // 创建一个测试用的具体节点类
        private class TestFlowNode : FlowBaseNode
        {
            public FlowEndpoint TestEndpoint { get; set; }
            
            public FlowOutEvent TestEmit()
            {
                return Emit(() => TestEndpoint);
            }
        }

        [Fact]
        public void FlowBaseNode_Inject_ShouldNotThrowException()
        {
            // Arrange
            var node = new TestFlowNode();
            var context = new NodeContext();
            
            // Act & Assert
            // Inject方法应该不会抛出异常
            var exception = Record.Exception(() => node.Inject(context));
            Assert.Null(exception);
        }

        [Fact]
        public void FlowBaseNode_Emit_ShouldReturnFlowOutEvent()
        {
            // Arrange
            var node = new TestFlowNode();
            var context = new NodeContext();
            node.Inject(context);
            
            // Act
            var result = node.TestEmit();
            
            // Assert
            Assert.NotNull(result);
            Assert.IsType<FlowOutEvent>(result);
        }

        [Fact]
        public void FlowBaseNode_Emit_ShouldReturnCorrectExpression()
        {
            // Arrange
            var node = new TestFlowNode();
            var context = new NodeContext();
            node.Inject(context);
            var expectedEndpoint = new FlowEndpoint();
            node.TestEndpoint = expectedEndpoint;
            
            // Act
            var result = node.TestEmit();
            
            // Assert
            Assert.NotNull(result.Expression);
            var compiledExpression = result.Expression.Compile();
            Assert.Equal(expectedEndpoint, compiledExpression());
        }

        [Fact]
        public void FlowOutEvent_Of_ShouldCreateFlowOutEvent()
        {
            // Arrange
            Expression<Func<FlowEndpoint>> expression = () => new FlowEndpoint();
            
            // Act
            var result = FlowOutEvent.Of(expression);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(expression, result.Expression);
        }

        [Fact]
        public void FlowOutEvent_Invoke_ShouldCompileAndExecuteExpression()
        {
            // Arrange
            var expectedEndpoint = new FlowEndpoint();
            Expression<Func<FlowEndpoint>> expression = () => expectedEndpoint;
            var flowOutEvent = FlowOutEvent.Of(expression);
            
            // Act
            var result = flowOutEvent.Invoke();
            
            // Assert
            Assert.Equal(expectedEndpoint, result);
        }

        [Fact]
        public void FlowOutEvent_GetLabelAttribute_ShouldReturnNull_WhenNoAttribute()
        {
            // Arrange
            Expression<Func<FlowEndpoint>> expression = () => new FlowEndpoint();
            var flowOutEvent = FlowOutEvent.Of(expression);
            
            // Act
            var result = flowOutEvent.GetLabelAttribute();
            
            // Assert
            Assert.Null(result);
        }
    }
} 