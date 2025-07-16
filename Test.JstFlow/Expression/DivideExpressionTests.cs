using JstFlow.External.Expressions;
using Xunit;
using System;

namespace Test.JstFlow.Expression
{
    public class DivideExpressionTests
    {
        [Fact]
        public void DivideExpression_Int_ShouldDivideCorrectly()
        {
            // Arrange
            var expression = new DivideExpression<int>
            {
                Left = 10,
                Right = 2
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(5, result);
        }

        [Fact]
        public void DivideExpression_Double_ShouldDivideCorrectly()
        {
            // Arrange
            var expression = new DivideExpression<double>
            {
                Left = 10.5,
                Right = 2.5
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(4.2, result, 2);
        }

        [Fact]
        public void DivideExpression_Float_ShouldDivideCorrectly()
        {
            // Arrange
            var expression = new DivideExpression<float>
            {
                Left = 10.5f,
                Right = 2.5f
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(4.2f, result, 2);
        }

        [Fact]
        public void DivideExpression_Long_ShouldDivideCorrectly()
        {
            // Arrange
            var expression = new DivideExpression<long>
            {
                Left = 1000000L,
                Right = 2000000L
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(0L, result);
        }

        [Fact]
        public void DivideExpression_WithOne_ShouldReturnOriginalValue()
        {
            // Arrange
            var expression = new DivideExpression<int>
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
        public void DivideExpression_WithNegativeNumbers_ShouldDivideCorrectly()
        {
            // Arrange
            var expression = new DivideExpression<int>
            {
                Left = -10,
                Right = -2
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(5, result);
        }

        [Fact]
        public void DivideExpression_WithOneNegativeNumber_ShouldReturnNegativeResult()
        {
            // Arrange
            var expression = new DivideExpression<int>
            {
                Left = -10,
                Right = 2
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(-5, result);
        }

        [Fact]
        public void DivideExpression_ByZero_ShouldThrowDivideByZeroException()
        {
            // Arrange
            var expression = new DivideExpression<int>
            {
                Left = 10,
                Right = 0
            };

            // Act & Assert
            var exception = Assert.Throws<DivideByZeroException>(() => expression.Evaluate());
            Assert.Equal("除数不能为零", exception.Message);
        }

        [Fact]
        public void DivideExpression_DoubleByZero_ShouldThrowDivideByZeroException()
        {
            // Arrange
            var expression = new DivideExpression<double>
            {
                Left = 10.5,
                Right = 0.0
            };

            // Act & Assert
            var exception = Assert.Throws<DivideByZeroException>(() => expression.Evaluate());
            Assert.Equal("除数不能为零", exception.Message);
        }

        [Fact]
        public void DivideExpression_ZeroByNonZero_ShouldReturnZero()
        {
            // Arrange
            var expression = new DivideExpression<int>
            {
                Left = 0,
                Right = 5
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(0, result);
        }
    }
} 