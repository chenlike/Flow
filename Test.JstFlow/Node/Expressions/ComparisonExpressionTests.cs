using System;
using Xunit;
using JstFlow.External.Expressions;

namespace Test.JstFlow.Node.Expressions
{
    public class ComparisonExpressionTests
    {
        [Fact]
        public void GreaterThanExpression_ShouldReturnTrue_WhenLeftIsGreater()
        {
            var expr = new GreaterThanExpression<int> { Left = 10, Right = 5 };
            Assert.True(expr.Evaluate());
        }

        [Fact]
        public void GreaterThanExpression_ShouldReturnFalse_WhenLeftIsNotGreater()
        {
            var expr = new GreaterThanExpression<int> { Left = 5, Right = 10 };
            Assert.False(expr.Evaluate());
        }

        [Fact]
        public void GreaterThanOrEqualExpression_ShouldReturnTrue_WhenLeftIsGreaterOrEqual()
        {
            var expr = new GreaterThanOrEqualExpression<int> { Left = 10, Right = 5 };
            Assert.True(expr.Evaluate());
            
            expr.Left = 5;
            expr.Right = 5;
            Assert.True(expr.Evaluate());
        }

        [Fact]
        public void GreaterThanOrEqualExpression_ShouldReturnFalse_WhenLeftIsLess()
        {
            var expr = new GreaterThanOrEqualExpression<int> { Left = 5, Right = 10 };
            Assert.False(expr.Evaluate());
        }

        [Fact]
        public void LessThanExpression_ShouldReturnTrue_WhenLeftIsLess()
        {
            var expr = new LessThanExpression<int> { Left = 5, Right = 10 };
            Assert.True(expr.Evaluate());
        }

        [Fact]
        public void LessThanExpression_ShouldReturnFalse_WhenLeftIsNotLess()
        {
            var expr = new LessThanExpression<int> { Left = 10, Right = 5 };
            Assert.False(expr.Evaluate());
        }

        [Fact]
        public void LessThanOrEqualExpression_ShouldReturnTrue_WhenLeftIsLessOrEqual()
        {
            var expr = new LessThanOrEqualExpression<int> { Left = 5, Right = 10 };
            Assert.True(expr.Evaluate());
            
            expr.Left = 5;
            expr.Right = 5;
            Assert.True(expr.Evaluate());
        }

        [Fact]
        public void LessThanOrEqualExpression_ShouldReturnFalse_WhenLeftIsGreater()
        {
            var expr = new LessThanOrEqualExpression<int> { Left = 10, Right = 5 };
            Assert.False(expr.Evaluate());
        }

        [Fact]
        public void StringComparison_ShouldWorkCorrectly()
        {
            var greaterExpr = new GreaterThanExpression<string> { Left = "zebra", Right = "apple" };
            Assert.True(greaterExpr.Evaluate());
            
            var lessExpr = new LessThanExpression<string> { Left = "apple", Right = "zebra" };
            Assert.True(lessExpr.Evaluate());
        }
    }
} 