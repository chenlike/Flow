using JstFlow.External.Expressions;
using Xunit;
using System;

namespace Test.JstFlow.Expression
{
    public class AddExpressionTests
    {
        [Fact]
        public void AddExpression_Int_ShouldAddCorrectly()
        {
            // Arrange
            var expression = new AddExpression<int>
            {
                Left = 5,
                Right = 3
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(8, result);
        }

        [Fact]
        public void AddExpression_Double_ShouldAddCorrectly()
        {
            // Arrange
            var expression = new AddExpression<double>
            {
                Left = 5.5,
                Right = 3.2
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(8.7, result, 2);
        }

        [Fact]
        public void AddExpression_Float_ShouldAddCorrectly()
        {
            // Arrange
            var expression = new AddExpression<float>
            {
                Left = 5.5f,
                Right = 3.2f
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(8.7f, result, 2);
        }

        [Fact]
        public void AddExpression_Long_ShouldAddCorrectly()
        {
            // Arrange
            var expression = new AddExpression<long>
            {
                Left = 1000000000L,
                Right = 2000000000L
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(3000000000L, result);
        }

        [Fact]
        public void AddExpression_WithZero_ShouldReturnOriginalValue()
        {
            // Arrange
            var expression = new AddExpression<int>
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
        public void AddExpression_WithNegativeNumbers_ShouldAddCorrectly()
        {
            // Arrange
            var expression = new AddExpression<int>
            {
                Left = -5,
                Right = -3
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(-8, result);
        }
    }
} 