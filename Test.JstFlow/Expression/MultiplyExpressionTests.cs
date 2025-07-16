using JstFlow.External.Expressions;
using Xunit;
using System;

namespace Test.JstFlow.Expression
{
    public class MultiplyExpressionTests
    {
        [Fact]
        public void MultiplyExpression_Int_ShouldMultiplyCorrectly()
        {
            // Arrange
            var expression = new MultiplyExpression<int>
            {
                Left = 5,
                Right = 3
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(15, result);
        }

        [Fact]
        public void MultiplyExpression_Double_ShouldMultiplyCorrectly()
        {
            // Arrange
            var expression = new MultiplyExpression<double>
            {
                Left = 5.5,
                Right = 3.2
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(17.6, result, 2);
        }

        [Fact]
        public void MultiplyExpression_Float_ShouldMultiplyCorrectly()
        {
            // Arrange
            var expression = new MultiplyExpression<float>
            {
                Left = 5.5f,
                Right = 3.2f
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(17.6f, result, 2);
        }

        [Fact]
        public void MultiplyExpression_Long_ShouldMultiplyCorrectly()
        {
            // Arrange
            var expression = new MultiplyExpression<long>
            {
                Left = 1000000L,
                Right = 2000000L
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(2000000000000L, result);
        }

        [Fact]
        public void MultiplyExpression_WithZero_ShouldReturnZero()
        {
            // Arrange
            var expression = new MultiplyExpression<int>
            {
                Left = 10,
                Right = 0
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void MultiplyExpression_WithOne_ShouldReturnOriginalValue()
        {
            // Arrange
            var expression = new MultiplyExpression<int>
            {
                Left = 10,
                Right = 1
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(10, result);
        }

        [Fact]
        public void MultiplyExpression_WithNegativeNumbers_ShouldMultiplyCorrectly()
        {
            // Arrange
            var expression = new MultiplyExpression<int>
            {
                Left = -5,
                Right = -3
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(15, result);
        }

        [Fact]
        public void MultiplyExpression_WithOneNegativeNumber_ShouldReturnNegativeResult()
        {
            // Arrange
            var expression = new MultiplyExpression<int>
            {
                Left = -5,
                Right = 3
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(-15, result);
        }
    }
} 