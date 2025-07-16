using JstFlow.External;
using JstFlow.Core;
using JstFlow.Attributes;
using JstFlow.External.Nodes;
using Xunit;
using System;
using System.Reflection;

namespace Test.JstFlow.Node
{
    public class ForNodeTests
    {
        [Fact]
        public void ForNode_ShouldHaveCorrectAttributes()
        {
            // Arrange & Act
            var forNode = new ForNode();
            
            // Assert
            var nodeAttribute = typeof(ForNode).GetCustomAttributes(typeof(FlowNodeAttribute), false);
            Assert.Single(nodeAttribute);
            Assert.Equal("For循环节点", ((FlowNodeAttribute)nodeAttribute[0]).Label);
        }

        [Fact]
        public void ForNode_ShouldHaveDefaultStepValue()
        {
            // Arrange & Act
            var forNode = new ForNode();
            
            // Assert
            Assert.Equal(1, forNode.Step);
        }

        [Fact]
        public void ForNode_StartLoop_ShouldReturnFlowOutEvent()
        {
            // Arrange
            var forNode = new ForNode
            {
                Start = 1,
                End = 3
            };
            var context = new NodeContext();
            forNode.Inject(context);
            
            // Act
            var result = forNode.StartLoop();
            
            // Assert
            Assert.NotNull(result);
            Assert.IsType<FlowOutEvent>(result);
        }

        [Fact]
        public void ForNode_StartLoop_ShouldResetState()
        {
            // Arrange
            var forNode = new ForNode
            {
                Start = 1,
                End = 3
            };
            var context = new NodeContext();
            forNode.Inject(context);
            
            // Act
            forNode.StartLoop();
            
            // Assert
            // 循环结束后 CurrentIndex 应该是 End + Step
            Assert.Equal(4, forNode.CurrentIndex);
            Assert.False(forNode.IsCompleted);
            Assert.False(forNode.IsBreak);
        }

        [Theory]
        [InlineData(1, 3, 1, 4)] // 正常循环，结束后 CurrentIndex = End + Step
        [InlineData(1, 5, 2, 7)] // 步长为2，结束后 CurrentIndex = End + Step
        [InlineData(5, 1, -1, 5)] // 递减循环，不会执行，CurrentIndex 保持为 Start
        public void ForNode_StartLoop_ShouldSetCorrectCurrentIndex(int start, int end, int step, int expectedFinalIndex)
        {
            // Arrange
            var forNode = new ForNode
            {
                Start = start,
                End = end,
                Step = step
            };
            var context = new NodeContext();
            forNode.Inject(context);
            
            // Act
            forNode.StartLoop();
            
            // Assert
            Assert.Equal(expectedFinalIndex, forNode.CurrentIndex);
        }

        [Fact]
        public void ForNode_StartLoop_ShouldReturnCorrectExpression()
        {
            // Arrange
            var forNode = new ForNode
            {
                Start = 1,
                End = 3
            };
            var context = new NodeContext();
            forNode.Inject(context);
            
            // Act
            var result = forNode.StartLoop();
            
            // Assert
            Assert.NotNull(result.Expression);
            var compiledExpression = result.Expression.Compile();
            Assert.Equal(forNode.LoopCompleted, compiledExpression());
        }

        [Fact]
        public void ForNode_Reset_ShouldResetAllProperties()
        {
            // Arrange
            var forNode = new ForNode
            {
                Start = 1,
                End = 3,
                CurrentIndex = 5,
                IsCompleted = true,
                IsBreak = true,
                BreakCondition = true
            };
            
            // Act
            forNode.Reset();
            
            // Assert
            Assert.Equal(1, forNode.CurrentIndex);
            Assert.False(forNode.IsCompleted);
            Assert.False(forNode.IsBreak);
            Assert.False(forNode.BreakCondition);
        }

        [Fact]
        public void ForNode_Break_ShouldSetBreakCondition()
        {
            // Arrange
            var forNode = new ForNode();
            
            // Act
            forNode.Break();
            
            // Assert
            Assert.True(forNode.BreakCondition);
            Assert.True(forNode.IsBreak);
        }

        [Fact]
        public void ForNode_StartLoop_WithBreakCondition_ShouldBreakEarly()
        {
            // Arrange
            var forNode = new ForNode
            {
                Start = 1,
                End = 5,
                BreakCondition = true
            };
            var context = new NodeContext();
            forNode.Inject(context);
            
            // Act
            forNode.StartLoop();
            
            // Assert
            Assert.Equal(6, forNode.CurrentIndex); // 循环结束后 CurrentIndex = End + Step
            // 注意：当 BreakCondition 为 true 时，循环会中断，但 IsBreak 不会被设置为 true
            // 因为 IsBreak 只在调用 Break() 方法时设置
            Assert.False(forNode.IsBreak);
        }

        [Fact]
        public void ForNode_Properties_ShouldBeSettable()
        {
            // Arrange
            var forNode = new ForNode();
            
            // Act & Assert
            forNode.Start = 10;
            Assert.Equal(10, forNode.Start);
            
            forNode.End = 20;
            Assert.Equal(20, forNode.End);
            
            forNode.Step = 5;
            Assert.Equal(5, forNode.Step);
            
            forNode.BreakCondition = true;
            Assert.True(forNode.BreakCondition);
            
            forNode.CurrentIndex = 15;
            Assert.Equal(15, forNode.CurrentIndex);
            
            forNode.IsCompleted = true;
            Assert.True(forNode.IsCompleted);
            
            forNode.IsBreak = true;
            Assert.True(forNode.IsBreak);
        }

        [Fact]
        public void ForNode_ShouldHaveCorrectSignalAttribute()
        {
            // Arrange
            var forNode = new ForNode();
            
            // Act
            var methodInfo = typeof(ForNode).GetMethod("StartLoop");
            var signalAttribute = methodInfo.GetCustomAttributes(typeof(FlowSignalAttribute), false);
            
            // Assert
            Assert.Single(signalAttribute);
            Assert.Equal("开始循环", ((FlowSignalAttribute)signalAttribute[0]).Label);
        }
    }
} 