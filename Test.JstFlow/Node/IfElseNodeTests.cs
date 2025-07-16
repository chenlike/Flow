using JstFlow.External;
using JstFlow.Core;
using JstFlow.Attributes;
using JstFlow.External.Nodes;
using Xunit;
using System;
using System.Reflection;

namespace Test.JstFlow.Node
{
    public class IfElseNodeTests
    {
        [Fact]
        public void IfElseNode_ShouldHaveCorrectAttributes()
        {
            // Arrange & Act
            var ifElseNode = new IfElseNode();
            
            // Assert
            var nodeAttribute = typeof(IfElseNode).GetCustomAttributes(typeof(FlowNodeAttribute), false);
            Assert.Single(nodeAttribute);
            Assert.Equal("If-Else条件节点", ((FlowNodeAttribute)nodeAttribute[0]).Label);
        }

        [Fact]
        public void IfElseNode_ShouldHaveRequiredProperties()
        {
            // Arrange
            var ifElseNode = new IfElseNode();
            
            // Act & Assert
            // 属性应该存在，但可能为 null
            Assert.True(typeof(IfElseNode).GetProperty("TrueBranch") != null);
            Assert.True(typeof(IfElseNode).GetProperty("FalseBranch") != null);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IfElseNode_Execute_ShouldReturnCorrectBranch(bool condition)
        {
            // Arrange
            var ifElseNode = new IfElseNode
            {
                Condition = condition
            };
            var context = new NodeContext();
            ifElseNode.Inject(context);
            
            // Act
            var result = ifElseNode.Execute();
            
            // Assert
            Assert.NotNull(result);
            Assert.IsType<FlowOutEvent>(result);
            
            var compiledExpression = result.Expression.Compile();
            var expectedEndpoint = condition ? ifElseNode.TrueBranch : ifElseNode.FalseBranch;
            Assert.Equal(expectedEndpoint, compiledExpression());
        }

        [Fact]
        public void IfElseNode_Execute_WhenConditionIsTrue_ShouldReturnTrueBranch()
        {
            // Arrange
            var ifElseNode = new IfElseNode
            {
                Condition = true
            };
            var context = new NodeContext();
            ifElseNode.Inject(context);
            
            // Act
            var result = ifElseNode.Execute();
            
            // Assert
            var compiledExpression = result.Expression.Compile();
            Assert.Equal(ifElseNode.TrueBranch, compiledExpression());
        }

        [Fact]
        public void IfElseNode_Execute_WhenConditionIsFalse_ShouldReturnFalseBranch()
        {
            // Arrange
            var ifElseNode = new IfElseNode
            {
                Condition = false
            };
            var context = new NodeContext();
            ifElseNode.Inject(context);
            
            // Act
            var result = ifElseNode.Execute();
            
            // Assert
            var compiledExpression = result.Expression.Compile();
            Assert.Equal(ifElseNode.FalseBranch, compiledExpression());
        }

        [Fact]
        public void IfElseNode_ShouldHaveCorrectSignalAttribute()
        {
            // Arrange
            var ifElseNode = new IfElseNode();
            
            // Act
            var methodInfo = typeof(IfElseNode).GetMethod("Execute");
            var signalAttribute = methodInfo.GetCustomAttributes(typeof(FlowSignalAttribute), false);
            
            // Assert
            Assert.Single(signalAttribute);
            Assert.Equal("执行", ((FlowSignalAttribute)signalAttribute[0]).Label);
        }

        [Fact]
        public void IfElseNode_ConditionProperty_ShouldBeSettable()
        {
            // Arrange
            var ifElseNode = new IfElseNode();
            var expectedCondition = true;
            
            // Act
            ifElseNode.Condition = expectedCondition;
            
            // Assert
            Assert.Equal(expectedCondition, ifElseNode.Condition);
        }
    }
} 