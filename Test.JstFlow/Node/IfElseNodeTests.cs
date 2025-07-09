using System;
using System.Collections.Generic;
using Xunit;
using JstFlow.External;
using JstFlow.Internal;
using JstFlow.Internal.NodeMeta;
using JstFlow.Internal.Metas;

namespace Test.JstFlow.Node
{
    public class IfElseNodeTests
    {
        [Fact]
        public void IfElseNode_ExecuteCondition_WithTrueCondition_ShouldInvokeTrueBranch()
        {
            // Arrange
            var ifElseNode = new IfElseNode();
            var trueBranchInvoked = false;
            var falseBranchInvoked = false;
            
            ifElseNode.Condition = true;
            ifElseNode.TrueBranch += () => trueBranchInvoked = true;
            ifElseNode.FalseBranch += () => falseBranchInvoked = true;

            // Act
            ifElseNode.ExecuteCondition();

            // Assert
            Assert.True(trueBranchInvoked);
            Assert.False(falseBranchInvoked);
        }

        [Fact]
        public void IfElseNode_ExecuteCondition_WithFalseCondition_ShouldInvokeFalseBranch()
        {
            // Arrange
            var ifElseNode = new IfElseNode();
            var trueBranchInvoked = false;
            var falseBranchInvoked = false;
            
            ifElseNode.Condition = false;
            ifElseNode.TrueBranch += () => trueBranchInvoked = true;
            ifElseNode.FalseBranch += () => falseBranchInvoked = true;

            // Act
            ifElseNode.ExecuteCondition();

            // Assert
            Assert.False(trueBranchInvoked);
            Assert.True(falseBranchInvoked);
        }

        [Fact]
        public void IfElseNode_ExecuteCondition_WithTrueCondition_WithoutEventHandlers_ShouldNotThrow()
        {
            // Arrange
            var ifElseNode = new IfElseNode();
            ifElseNode.Condition = true;

            // Act & Assert
            var exception = Record.Exception(() => ifElseNode.ExecuteCondition());
            Assert.Null(exception);
        }

        [Fact]
        public void IfElseNode_ExecuteCondition_WithFalseCondition_WithoutEventHandlers_ShouldNotThrow()
        {
            // Arrange
            var ifElseNode = new IfElseNode();
            ifElseNode.Condition = false;

            // Act & Assert
            var exception = Record.Exception(() => ifElseNode.ExecuteCondition());
            Assert.Null(exception);
        }

        [Fact]
        public void IfElseNode_ExecuteCondition_WithMultipleHandlers_ShouldInvokeAll()
        {
            // Arrange
            var ifElseNode = new IfElseNode();
            var handler1Invoked = false;
            var handler2Invoked = false;
            
            ifElseNode.Condition = true;
            ifElseNode.TrueBranch += () => handler1Invoked = true;
            ifElseNode.TrueBranch += () => handler2Invoked = true;

            // Act
            ifElseNode.ExecuteCondition();

            // Assert
            Assert.True(handler1Invoked);
            Assert.True(handler2Invoked);
        }

        [Fact]
        public void IfElseNode_ExecuteCondition_WithDynamicConditionChange_ShouldUseCurrentValue()
        {
            // Arrange
            var ifElseNode = new IfElseNode();
            var trueBranchInvoked = false;
            var falseBranchInvoked = false;
            
            ifElseNode.TrueBranch += () => trueBranchInvoked = true;
            ifElseNode.FalseBranch += () => falseBranchInvoked = true;

            // Act - 先设置为true
            ifElseNode.Condition = true;
            ifElseNode.ExecuteCondition();

            // Assert - 应该执行true分支
            Assert.True(trueBranchInvoked);
            Assert.False(falseBranchInvoked);

            // 重置
            trueBranchInvoked = false;
            falseBranchInvoked = false;

            // Act - 再设置为false
            ifElseNode.Condition = false;
            ifElseNode.ExecuteCondition();

            // Assert - 应该执行false分支
            Assert.False(trueBranchInvoked);
            Assert.True(falseBranchInvoked);
        }

        [Fact]
        public void IfElseNode_NodeInfo_ShouldHaveCorrectMetadata()
        {
            // Arrange & Act
            var nodeInfo = NodeFactory.CreateNodeInfo(typeof(IfElseNode));

            // Assert
            Assert.Equal("IfElseNode", nodeInfo.Label.Code);
            Assert.Equal("If-Else条件节点", nodeInfo.Label.DisplayName);
            Assert.Equal(typeof(IfElseNode), nodeInfo.NodeImplType);
            Assert.Equal(NodeKind.Node, nodeInfo.Kind);

            // 验证输入字段
            Assert.Single(nodeInfo.InputFields);
            var conditionInput = nodeInfo.InputFields[0];
            Assert.Equal("Condition", conditionInput.Label.Code);
            Assert.Equal("条件", conditionInput.Label.DisplayName);
            Assert.True(conditionInput.Required);
            Assert.Equal("Boolean", conditionInput.Type);

            // 验证输出字段
            Assert.Empty(nodeInfo.OutputFields);

            // 验证信号
            Assert.Single(nodeInfo.Signals);
            var executeSignal = nodeInfo.Signals[0];
            Assert.Equal("ExecuteCondition", executeSignal.Label.Code);
            Assert.Equal("执行", executeSignal.Label.DisplayName);

            // 验证事件
            Assert.Equal(2, nodeInfo.Emits.Count);
            var trueBranchEmit = nodeInfo.Emits.Find(e => e.Label.Code == "TrueBranch");
            Assert.NotNull(trueBranchEmit);
            Assert.Equal("如果真，执行", trueBranchEmit.Label.DisplayName);

            var falseBranchEmit = nodeInfo.Emits.Find(e => e.Label.Code == "FalseBranch");
            Assert.NotNull(falseBranchEmit);
            Assert.Equal("如果假，执行", falseBranchEmit.Label.DisplayName);
        }

        [Fact]
        public void IfElseNode_ExecuteCondition_WithComplexLogic_ShouldWorkCorrectly()
        {
            // Arrange
            var ifElseNode = new IfElseNode();
            var result = "";
            
            ifElseNode.TrueBranch += () => result += "True";
            ifElseNode.FalseBranch += () => result += "False";

            // Act - 测试边界条件
            ifElseNode.Condition = true;
            ifElseNode.ExecuteCondition();
            ifElseNode.ExecuteCondition(); // 多次执行

            // Assert
            Assert.Equal("TrueTrue", result);
        }

        [Fact]
        public void IfElseNode_ExecuteCondition_WithEventRemoval_ShouldWorkCorrectly()
        {
            // Arrange
            var ifElseNode = new IfElseNode();
            var invocationCount = 0;
            
            Action handler = () => invocationCount++;
            ifElseNode.Condition = true;
            ifElseNode.TrueBranch += handler;

            // Act
            ifElseNode.ExecuteCondition();
            ifElseNode.TrueBranch -= handler;
            ifElseNode.ExecuteCondition();

            // Assert
            Assert.Equal(1, invocationCount);
        }
    }
} 