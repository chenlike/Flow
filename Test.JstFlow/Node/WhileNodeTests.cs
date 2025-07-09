using System;
using System.Collections.Generic;
using Xunit;
using JstFlow.External;
using JstFlow.Internal;
using JstFlow.Internal.NodeMeta;
using JstFlow.Internal.Metas;

namespace Test.JstFlow.Node
{
    public class WhileNodeTests
    {
        [Fact]
        public void WhileNode_StartLoop_WithTrueCondition_ShouldExecuteLoopBody()
        {
            // Arrange
            var whileNode = new WhileNode();
            var executionCount = 0;
            
            whileNode.Condition = true;
            whileNode.LoopBody += () => 
            {
                executionCount++;
                whileNode.Condition = false; // 第一次执行后停止循环
            };

            // Act
            whileNode.StartLoop();

            // Assert
            Assert.True(whileNode.IsCompleted);
            Assert.Equal(1, whileNode.CurrentIteration);
            Assert.Equal(1, executionCount);
        }

        [Fact]
        public void WhileNode_StartLoop_WithFalseCondition_ShouldNotExecuteLoopBody()
        {
            // Arrange
            var whileNode = new WhileNode();
            var executionCount = 0;
            
            whileNode.Condition = false;
            whileNode.LoopBody += () => executionCount++;

            // Act
            whileNode.StartLoop();

            // Assert
            Assert.True(whileNode.IsCompleted);
            Assert.Equal(0, whileNode.CurrentIteration);
            Assert.Equal(0, executionCount);
        }

        [Fact]
        public void WhileNode_StartLoop_WithMultipleIterations_ShouldExecuteCorrectTimes()
        {
            // Arrange
            var whileNode = new WhileNode();
            var executionCount = 0;
            var iterationCount = 0;
            
            whileNode.Condition = true;
            whileNode.LoopBody += () => 
            {
                executionCount++;
                iterationCount++;
                if (iterationCount >= 5)
                {
                    whileNode.Condition = false;
                }
            };

            // Act
            whileNode.StartLoop();

            // Assert
            Assert.True(whileNode.IsCompleted);
            Assert.Equal(5, whileNode.CurrentIteration);
            Assert.Equal(5, executionCount);
        }

        [Fact]
        public void WhileNode_StartLoop_WithMaxIterations_ShouldStopAtMax()
        {
            // Arrange
            var whileNode = new WhileNode();
            var executionCount = 0;
            
            whileNode.Condition = true;
            whileNode.MaxIterations = 3;
            whileNode.LoopBody += () => executionCount++;

            // Act
            whileNode.StartLoop();

            // Assert
            Assert.True(whileNode.IsCompleted);
            Assert.Equal(3, whileNode.CurrentIteration);
            Assert.Equal(3, executionCount);
        }

        [Fact]
        public void WhileNode_StartLoop_ShouldInvokeLoopCompleted()
        {
            // Arrange
            var whileNode = new WhileNode();
            var completedInvoked = false;
            
            whileNode.Condition = false;
            whileNode.LoopCompleted += () => completedInvoked = true;

            // Act
            whileNode.StartLoop();

            // Assert
            Assert.True(completedInvoked);
        }

        [Fact]
        public void WhileNode_Reset_ShouldResetState()
        {
            // Arrange
            var whileNode = new WhileNode();
            whileNode.Condition = true;
            whileNode.StartLoop();

            // Act
            whileNode.Reset();

            // Assert
            Assert.Equal(0, whileNode.CurrentIteration);
            Assert.False(whileNode.IsCompleted);
        }

        [Fact]
        public void WhileNode_StopLoop_ShouldSetConditionToFalse()
        {
            // Arrange
            var whileNode = new WhileNode();
            whileNode.Condition = true;

            // Act
            whileNode.StopLoop();

            // Assert
            Assert.False(whileNode.Condition);
        }

        [Fact]
        public void WhileNode_StartLoop_WithStopLoop_ShouldStopImmediately()
        {
            // Arrange
            var whileNode = new WhileNode();
            var executionCount = 0;
            
            whileNode.Condition = true;
            whileNode.LoopBody += () => 
            {
                executionCount++;
                whileNode.StopLoop();
            };

            // Act
            whileNode.StartLoop();

            // Assert
            Assert.True(whileNode.IsCompleted);
            Assert.Equal(1, whileNode.CurrentIteration);
            Assert.Equal(1, executionCount);
            Assert.False(whileNode.Condition);
        }

        [Fact]
        public void WhileNode_NodeInfo_ShouldHaveCorrectMetadata()
        {
            // Arrange & Act
            var nodeInfo = NodeFactory.CreateNodeInfo(typeof(WhileNode));

            // Assert
            Assert.Equal("WhileNode", nodeInfo.Label.Code);
            Assert.Equal("While循环节点", nodeInfo.Label.DisplayName);
            Assert.Equal(typeof(WhileNode), nodeInfo.NodeImplType);
            Assert.Equal(NodeKind.Node, nodeInfo.Kind);

            // 验证输入字段
            Assert.Equal(2, nodeInfo.InputFields.Count);
            var conditionInput = nodeInfo.InputFields.Find(f => f.Label.Code == "Condition");
            Assert.NotNull(conditionInput);
            Assert.True(conditionInput.Required);
            Assert.Equal("Boolean", conditionInput.Type);

            var maxIterationsInput = nodeInfo.InputFields.Find(f => f.Label.Code == "MaxIterations");
            Assert.NotNull(maxIterationsInput);
            Assert.False(maxIterationsInput.Required);
            Assert.Equal("Int32", maxIterationsInput.Type);

            // 验证输出字段
            Assert.Equal(2, nodeInfo.OutputFields.Count);
            var currentIterationOutput = nodeInfo.OutputFields.Find(f => f.Label.Code == "CurrentIteration");
            Assert.NotNull(currentIterationOutput);
            Assert.Equal("Int32", currentIterationOutput.Type);

            var isCompletedOutput = nodeInfo.OutputFields.Find(f => f.Label.Code == "IsCompleted");
            Assert.NotNull(isCompletedOutput);
            Assert.Equal("Boolean", isCompletedOutput.Type);

            // 验证信号
            Assert.Equal(3, nodeInfo.Signals.Count);
            var startLoopSignal = nodeInfo.Signals.Find(s => s.Label.Code == "StartLoop");
            Assert.NotNull(startLoopSignal);

            var resetSignal = nodeInfo.Signals.Find(s => s.Label.Code == "Reset");
            Assert.NotNull(resetSignal);

            var stopLoopSignal = nodeInfo.Signals.Find(s => s.Label.Code == "StopLoop");
            Assert.NotNull(stopLoopSignal);

            // 验证事件
            Assert.Equal(2, nodeInfo.Emits.Count);
            var loopBodyEmit = nodeInfo.Emits.Find(e => e.Label.Code == "LoopBody");
            Assert.NotNull(loopBodyEmit);

            var loopCompletedEmit = nodeInfo.Emits.Find(e => e.Label.Code == "LoopCompleted");
            Assert.NotNull(loopCompletedEmit);
        }

        [Fact]
        public void WhileNode_StartLoop_WithInfiniteLoop_ShouldStopAtMaxIterations()
        {
            // Arrange
            var whileNode = new WhileNode();
            var executionCount = 0;
            
            whileNode.Condition = true;
            whileNode.MaxIterations = 10;
            whileNode.LoopBody += () => executionCount++;

            // Act
            whileNode.StartLoop();

            // Assert
            Assert.True(whileNode.IsCompleted);
            Assert.Equal(10, whileNode.CurrentIteration);
            Assert.Equal(10, executionCount);
        }

        [Fact]
        public void WhileNode_StartLoop_WithMultipleHandlers_ShouldInvokeAll()
        {
            // Arrange
            var whileNode = new WhileNode();
            var handler1Count = 0;
            var handler2Count = 0;
            
            whileNode.Condition = true;
            whileNode.LoopBody += () => 
            {
                handler1Count++;
                if (handler1Count >= 3)
                {
                    whileNode.Condition = false;
                }
            };
            whileNode.LoopBody += () => handler2Count++;

            // Act
            whileNode.StartLoop();

            // Assert
            Assert.Equal(3, handler1Count);
            Assert.Equal(3, handler2Count);
        }

        [Fact]
        public void WhileNode_StartLoop_WithLoopCompletedHandler_ShouldInvokeOnce()
        {
            // Arrange
            var whileNode = new WhileNode();
            var completedCount = 0;
            
            whileNode.Condition = false;
            whileNode.LoopCompleted += () => completedCount++;

            // Act
            whileNode.StartLoop();

            // Assert
            Assert.Equal(1, completedCount);
        }
    }
} 