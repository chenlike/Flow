using JstFlow.External.Expressions;
using Xunit;
using System;

namespace Test.JstFlow.Expression
{
    public class AndExpressionTests
    {
        [Fact]
        public void AndExpression_TrueAndTrue_ShouldReturnTrue()
        {
            // Arrange
            var expression = new AndExpression
            {
                Left = true,
                Right = true
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void AndExpression_TrueAndFalse_ShouldReturnFalse()
        {
            // Arrange
            var expression = new AndExpression
            {
                Left = true,
                Right = false
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void AndExpression_FalseAndTrue_ShouldReturnFalse()
        {
            // Arrange
            var expression = new AndExpression
            {
                Left = false,
                Right = true
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void AndExpression_FalseAndFalse_ShouldReturnFalse()
        {
            // Arrange
            var expression = new AndExpression
            {
                Left = false,
                Right = false
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void AndExpression_WithDefaultValues_ShouldReturnFalse()
        {
            // Arrange
            var expression = new AndExpression();

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void AndExpression_WithOnlyLeftSet_ShouldReturnFalse()
        {
            // Arrange
            var expression = new AndExpression
            {
                Left = true
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void AndExpression_WithOnlyRightSet_ShouldReturnFalse()
        {
            // Arrange
            var expression = new AndExpression
            {
                Right = true
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.False(result);
        }
    }
} 