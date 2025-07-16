using JstFlow.External.Expressions;
using Xunit;
using System;

namespace Test.JstFlow.Expression
{
    public class SubtractExpressionTests
    {
        [Fact]
        public void SubtractExpression_Int_ShouldSubtractCorrectly()
        {
            // Arrange
            var expression = new SubtractExpression<int>
            {
                Left = 10,
                Right = 3
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(7, result);
        }

        [Fact]
        public void SubtractExpression_Double_ShouldSubtractCorrectly()
        {
            // Arrange
            var expression = new SubtractExpression<double>
            {
                Left = 10.5,
                Right = 3.2
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(7.3, result, 2);
        }

        [Fact]
        public void SubtractExpression_Float_ShouldSubtractCorrectly()
        {
            // Arrange
            var expression = new SubtractExpression<float>
            {
                Left = 10.5f,
                Right = 3.2f
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(7.3f, result, 2);
        }

        [Fact]
        public void SubtractExpression_Long_ShouldSubtractCorrectly()
        {
            // Arrange
            var expression = new SubtractExpression<long>
            {
                Left = 5000000000L,
                Right = 2000000000L
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(3000000000L, result);
        }

        [Fact]
        public void SubtractExpression_WithZero_ShouldReturnOriginalValue()
        {
            // Arrange
            var expression = new SubtractExpression<int>
            {
                Left = 10,
                Right = 0
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(10, result);
        }

        [Fact]
        public void SubtractExpression_WithNegativeNumbers_ShouldSubtractCorrectly()
        {
            // Arrange
            var expression = new SubtractExpression<int>
            {
                Left = -5,
                Right = -3
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(-2, result);
        }

        [Fact]
        public void SubtractExpression_SubtractFromZero_ShouldReturnNegativeValue()
        {
            // Arrange
            var expression = new SubtractExpression<int>
            {
                Left = 0,
                Right = 5
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(-5, result);
        }
    }
} 