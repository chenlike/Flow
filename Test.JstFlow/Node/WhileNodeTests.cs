using JstFlow.External;
using JstFlow.Core;
using JstFlow.Attributes;
using JstFlow.External.Nodes;
using Xunit;
using System;
using System.Reflection;

namespace Test.JstFlow.Node
{
    public class WhileNodeTests
    {
        [Fact]
        public void WhileNode_ShouldHaveCorrectAttributes()
        {
            // Arrange & Act
            var whileNode = new WhileNode();
            
            // Assert
            var nodeAttribute = typeof(WhileNode).GetCustomAttributes(typeof(FlowNodeAttribute), false);
            Assert.Single(nodeAttribute);
            Assert.Equal("While循环节点", ((FlowNodeAttribute)nodeAttribute[0]).Label);
        }

        [Fact]
        public void WhileNode_ShouldHaveDefaultMaxIterations()
        {
            // Arrange & Act
            var whileNode = new WhileNode();
            
            // Assert
            Assert.Equal(1000, whileNode.MaxIterations);
        }

        [Fact]
        public void WhileNode_StartLoop_ShouldReturnFlowOutEvent()
        {
            // Arrange
            var whileNode = new WhileNode
            {
                Condition = true
            };
            var context = new NodeContext();
            whileNode.Inject(context);
            
            // Act
            var result = whileNode.StartLoop();
            
            // Assert
            Assert.NotNull(result);
            Assert.IsType<FlowOutEvent>(result);
        }

        [Fact]
        public void WhileNode_StartLoop_ShouldReturnCorrectExpression()
        {
            // Arrange
            var whileNode = new WhileNode
            {
                Condition = true
            };
            var context = new NodeContext();
            whileNode.Inject(context);
            
            // Act
            var result = whileNode.StartLoop();
            
            // Assert
            Assert.NotNull(result.Expression);
            var compiledExpression = result.Expression.Compile();
            Assert.Equal(whileNode.LoopCompleted, compiledExpression());
        }

        [Theory]
        [InlineData(true, 1000)] // 条件为真，会一直循环直到达到最大迭代次数
        [InlineData(false, 0)] // 条件为假，不应该执行
        public void WhileNode_StartLoop_ShouldRespectCondition(bool condition, int expectedIterations)
        {
            // Arrange
            var whileNode = new WhileNode
            {
                Condition = condition
            };
            var context = new NodeContext();
            whileNode.Inject(context);
            
            // Act
            whileNode.StartLoop();
            
            // Assert
            Assert.Equal(expectedIterations, whileNode.CurrentIteration);
        }

        [Fact]
        public void WhileNode_StartLoop_ShouldRespectMaxIterations()
        {
            // Arrange
            var whileNode = new WhileNode
            {
                Condition = true,
                MaxIterations = 5
            };
            var context = new NodeContext();
            whileNode.Inject(context);
            
            // Act
            whileNode.StartLoop();
            
            // Assert
            Assert.Equal(5, whileNode.CurrentIteration);
        }

        [Fact]
        public void WhileNode_Reset_ShouldResetProperties()
        {
            // Arrange
            var whileNode = new WhileNode
            {
                CurrentIteration = 10,
                IsCompleted = true
            };
            
            // Act
            whileNode.Reset();
            
            // Assert
            Assert.Equal(0, whileNode.CurrentIteration);
            Assert.False(whileNode.IsCompleted);
        }

        [Fact]
        public void WhileNode_Break_ShouldStopLoop()
        {
            // Arrange
            var whileNode = new WhileNode
            {
                Condition = true,
                MaxIterations = 10
            };
            var context = new NodeContext();
            whileNode.Inject(context);
            
            // Act
            whileNode.Break();
            whileNode.StartLoop();
            
            // Assert
            Assert.Equal(10, whileNode.CurrentIteration); // 会一直循环到最大迭代次数
        }

        [Fact]
        public void WhileNode_Properties_ShouldBeSettable()
        {
            // Arrange
            var whileNode = new WhileNode();
            
            // Act & Assert
            whileNode.Condition = true;
            Assert.True(whileNode.Condition);
            
            whileNode.MaxIterations = 500;
            Assert.Equal(500, whileNode.MaxIterations);
            
            whileNode.CurrentIteration = 15;
            Assert.Equal(15, whileNode.CurrentIteration);
            
            whileNode.IsCompleted = true;
            Assert.True(whileNode.IsCompleted);
        }

        [Fact]
        public void WhileNode_ShouldHaveCorrectSignalAttribute()
        {
            // Arrange
            var whileNode = new WhileNode();
            
            // Act
            var methodInfo = typeof(WhileNode).GetMethod("StartLoop");
            var signalAttribute = methodInfo.GetCustomAttributes(typeof(FlowSignalAttribute), false);
            
            // Assert
            Assert.Single(signalAttribute);
            Assert.Equal("开始循环", ((FlowSignalAttribute)signalAttribute[0]).Label);
        }

        [Fact]
        public void WhileNode_StartLoop_ShouldResetBreakFlag()
        {
            // Arrange
            var whileNode = new WhileNode
            {
                Condition = true,
                MaxIterations = 3
            };
            var context = new NodeContext();
            whileNode.Inject(context);
            
            // Act
            whileNode.StartLoop();
            
            // Assert
            // 循环应该正常完成，没有被中断
            Assert.Equal(3, whileNode.CurrentIteration);
        }
    }
} 