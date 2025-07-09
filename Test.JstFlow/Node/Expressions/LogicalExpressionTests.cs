using System;
using Xunit;
using JstFlow.External.Expressions;

namespace Test.JstFlow.Node.Expressions
{
    public class LogicalExpressionTests
    {
        [Fact]
        public void AndExpression_ShouldReturnTrue_WhenBothOperandsAreTrue()
        {
            var expr = new AndExpression { Left = true, Right = true };
            Assert.True(expr.Evaluate());
        }

        [Fact]
        public void AndExpression_ShouldReturnFalse_WhenLeftOperandIsFalse()
        {
            var expr = new AndExpression { Left = false, Right = true };
            Assert.False(expr.Evaluate());
        }

        [Fact]
        public void AndExpression_ShouldReturnFalse_WhenRightOperandIsFalse()
        {
            var expr = new AndExpression { Left = true, Right = false };
            Assert.False(expr.Evaluate());
        }

        [Fact]
        public void AndExpression_ShouldReturnFalse_WhenBothOperandsAreFalse()
        {
            var expr = new AndExpression { Left = false, Right = false };
            Assert.False(expr.Evaluate());
        }

        [Fact]
        public void OrExpression_ShouldReturnTrue_WhenBothOperandsAreTrue()
        {
            var expr = new OrExpression { Left = true, Right = true };
            Assert.True(expr.Evaluate());
        }

        [Fact]
        public void OrExpression_ShouldReturnTrue_WhenLeftOperandIsTrue()
        {
            var expr = new OrExpression { Left = true, Right = false };
            Assert.True(expr.Evaluate());
        }

        [Fact]
        public void OrExpression_ShouldReturnTrue_WhenRightOperandIsTrue()
        {
            var expr = new OrExpression { Left = false, Right = true };
            Assert.True(expr.Evaluate());
        }

        [Fact]
        public void OrExpression_ShouldReturnFalse_WhenBothOperandsAreFalse()
        {
            var expr = new OrExpression { Left = false, Right = false };
            Assert.False(expr.Evaluate());
        }

        [Fact]
        public void LogicalExpressions_ShouldWorkWithAllCombinations()
        {
            // Test all combinations for AND
            var andExpr = new AndExpression();
            
            andExpr.Left = true; andExpr.Right = true;
            Assert.True(andExpr.Evaluate());
            
            andExpr.Left = true; andExpr.Right = false;
            Assert.False(andExpr.Evaluate());
            
            andExpr.Left = false; andExpr.Right = true;
            Assert.False(andExpr.Evaluate());
            
            andExpr.Left = false; andExpr.Right = false;
            Assert.False(andExpr.Evaluate());

            // Test all combinations for OR
            var orExpr = new OrExpression();
            
            orExpr.Left = true; orExpr.Right = true;
            Assert.True(orExpr.Evaluate());
            
            orExpr.Left = true; orExpr.Right = false;
            Assert.True(orExpr.Evaluate());
            
            orExpr.Left = false; orExpr.Right = true;
            Assert.True(orExpr.Evaluate());
            
            orExpr.Left = false; orExpr.Right = false;
            Assert.False(orExpr.Evaluate());
        }
    }
} 