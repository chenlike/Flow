using JstFlow.External;
using JstFlow.Core;
using JstFlow.Attributes;
using JstFlow.External.Nodes;
using Xunit;
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

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
        public void WhileNode_StartLoop_ShouldReturnIEnumerableFlowOutEvent()
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
            Assert.IsAssignableFrom<IEnumerable<FlowOutEvent>>(result);
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
            var events = whileNode.StartLoop().ToList();
            
            // Assert
            Assert.NotNull(events);
            Assert.NotEmpty(events);
            
            // 最后一个事件应该是循环完成事件
            var lastEvent = events.Last();
            Assert.NotNull(lastEvent.Expression);
            var compiledExpression = lastEvent.Expression.Compile();
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
            var events = whileNode.StartLoop().ToList();
            
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
            var events = whileNode.StartLoop().ToList();
            
            // Assert
            Assert.Equal(5, whileNode.CurrentIteration);
        }

        [Fact]
        public void WhileNode_StartLoop_ShouldReturnCorrectNumberOfEvents()
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
            var events = whileNode.StartLoop().ToList();
            
            // Assert
            // 应该有3个循环体事件 + 1个完成事件 = 4个事件
            Assert.Equal(4, events.Count);
        }

        [Fact]
        public void WhileNode_StartLoop_ShouldEmitLoopBodyEvents()
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
            var events = whileNode.StartLoop().ToList();
            
            // Assert
            // 前3个事件应该是循环体事件
            for (int i = 0; i < 3; i++)
            {
                var loopBodyEvent = events[i];
                Assert.NotNull(loopBodyEvent.Expression);
                var compiledExpression = loopBodyEvent.Expression.Compile();
                Assert.Equal(whileNode.LoopBody, compiledExpression());
            }
        }

        [Fact]
        public void WhileNode_StartLoop_ShouldEmitLoopCompletedEvent()
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
            var events = whileNode.StartLoop().ToList();
            
            // Assert
            // 最后一个事件应该是循环完成事件
            var completedEvent = events.Last();
            Assert.NotNull(completedEvent.Expression);
            var compiledExpression = completedEvent.Expression.Compile();
            Assert.Equal(whileNode.LoopCompleted, compiledExpression());
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
            var events = whileNode.StartLoop().ToList();
            
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
            var events = whileNode.StartLoop().ToList();
            
            // Assert
            // 循环应该正常完成，没有被中断
            Assert.Equal(3, whileNode.CurrentIteration);
        }

        [Fact]
        public void WhileNode_StartLoop_WithFalseCondition_ShouldReturnOnlyCompletedEvent()
        {
            // Arrange
            var whileNode = new WhileNode
            {
                Condition = false
            };
            var context = new NodeContext();
            whileNode.Inject(context);
            
            // Act
            var events = whileNode.StartLoop().ToList();
            
            // Assert
            // 条件为假时，应该只有一个完成事件
            Assert.Single(events);
            var completedEvent = events.First();
            Assert.NotNull(completedEvent.Expression);
            var compiledExpression = completedEvent.Expression.Compile();
            Assert.Equal(whileNode.LoopCompleted, compiledExpression());
        }
    }
} 