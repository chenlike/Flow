using JstFlow.External.Expressions;
using Xunit;
using System;

namespace Test.JstFlow.Expression
{
    public class OrExpressionTests
    {
        [Fact]
        public void OrExpression_TrueOrTrue_ShouldReturnTrue()
        {
            // Arrange
            var expression = new OrExpression
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
        public void OrExpression_TrueOrFalse_ShouldReturnTrue()
        {
            // Arrange
            var expression = new OrExpression
            {
                Left = true,
                Right = false
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void OrExpression_FalseOrTrue_ShouldReturnTrue()
        {
            // Arrange
            var expression = new OrExpression
            {
                Left = false,
                Right = true
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void OrExpression_FalseOrFalse_ShouldReturnFalse()
        {
            // Arrange
            var expression = new OrExpression
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
        public void OrExpression_WithDefaultValues_ShouldReturnFalse()
        {
            // Arrange
            var expression = new OrExpression();

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void OrExpression_WithOnlyLeftTrue_ShouldReturnTrue()
        {
            // Arrange
            var expression = new OrExpression
            {
                Left = true
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void OrExpression_WithOnlyRightTrue_ShouldReturnTrue()
        {
            // Arrange
            var expression = new OrExpression
            {
                Right = true
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void OrExpression_WithOnlyLeftFalse_ShouldReturnFalse()
        {
            // Arrange
            var expression = new OrExpression
            {
                Left = false
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void OrExpression_WithOnlyRightFalse_ShouldReturnFalse()
        {
            // Arrange
            var expression = new OrExpression
            {
                Right = false
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.False(result);
        }
    }
} 