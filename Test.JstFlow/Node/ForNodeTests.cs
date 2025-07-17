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
        public void ForNode_StartLoop_ShouldReturnIEnumerableFlowOutEvent()
        {
            // Arrange
            var forNode = new ForNode
            {
                Start = 1,
                End = 3
            };
            
            // Act
            var result = forNode.StartLoop();
            
            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<FlowOutEvent>>(result);
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
            
            // Act
            var events = forNode.StartLoop().ToList();
            
            // Assert
            // 循环结束后 CurrentIndex 应该是 End + Step
            Assert.Equal(4, forNode.CurrentIndex);
            Assert.False(forNode.IsCompleted);
            
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
            
            // Act
            var events = forNode.StartLoop().ToList();
            
            // Assert
            Assert.Equal(expectedFinalIndex, forNode.CurrentIndex);
        }

        [Fact]
        public void ForNode_StartLoop_ShouldReturnCorrectNumberOfEvents()
        {
            // Arrange
            var forNode = new ForNode
            {
                Start = 1,
                End = 3
            };
            
            // Act
            var events = forNode.StartLoop().ToList();
            
            // Assert
            // 应该有3个循环体事件 + 1个完成事件 = 4个事件
            Assert.Equal(4, events.Count);
        }

        [Fact]
        public void ForNode_StartLoop_ShouldReturnCorrectMemberName()
        {
            // Arrange
            var forNode = new ForNode
            {
                Start = 1,
                End = 3
            };
            
            // Act
            var events = forNode.StartLoop().ToList();
            
            // Assert
            Assert.NotNull(events);
            Assert.NotEmpty(events);
            
            // 最后一个事件应该是循环完成事件
            var lastEvent = events.Last();
            Assert.Equal("LoopCompleted", lastEvent.MemberName);
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
            };
            
            // Act
            forNode.Reset();
            
            // Assert
            Assert.Equal(1, forNode.CurrentIndex);
            Assert.False(forNode.IsCompleted);
            
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

            
            forNode.CurrentIndex = 15;
            Assert.Equal(15, forNode.CurrentIndex);
            
            forNode.IsCompleted = true;
            Assert.True(forNode.IsCompleted);

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

        [Fact]
        public void ForNode_StartLoop_ShouldEmitLoopBodyEvents()
        {
            // Arrange
            var forNode = new ForNode
            {
                Start = 1,
                End = 3
            };
            
            // Act
            var events = forNode.StartLoop().ToList();
            
            // Assert
            // 前3个事件应该是循环体事件
            for (int i = 0; i < 3; i++)
            {
                var loopBodyEvent = events[i];
                Assert.Equal("LoopBody", loopBodyEvent.MemberName);
            }
        }

        [Fact]
        public void ForNode_StartLoop_ShouldEmitLoopCompletedEvent()
        {
            // Arrange
            var forNode = new ForNode
            {
                Start = 1,
                End = 3
            };
            
            // Act
            var events = forNode.StartLoop().ToList();
            
            // Assert
            // 最后一个事件应该是循环完成事件
            var completedEvent = events.Last();
            Assert.Equal("LoopCompleted", completedEvent.MemberName);
        }
    }
} 