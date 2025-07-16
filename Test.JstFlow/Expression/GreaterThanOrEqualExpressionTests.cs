using JstFlow.External.Expressions;
using Xunit;
using System;

namespace Test.JstFlow.Expression
{
    public class GreaterThanOrEqualExpressionTests
    {
        [Fact]
        public void GreaterThanOrEqualExpression_Int_LeftGreaterThanRight_ShouldReturnTrue()
        {
            // Arrange
            var expression = new GreaterThanOrEqualExpression<int>
            {
                Left = 10,
                Right = 5
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GreaterThanOrEqualExpression_Int_LeftEqualToRight_ShouldReturnTrue()
        {
            // Arrange
            var expression = new GreaterThanOrEqualExpression<int>
            {
                Left = 5,
                Right = 5
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GreaterThanOrEqualExpression_Int_LeftLessThanRight_ShouldReturnFalse()
        {
            // Arrange
            var expression = new GreaterThanOrEqualExpression<int>
            {
                Left = 3,
                Right = 5
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GreaterThanOrEqualExpression_Double_LeftGreaterThanRight_ShouldReturnTrue()
        {
            // Arrange
            var expression = new GreaterThanOrEqualExpression<double>
            {
                Left = 10.5,
                Right = 5.2
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GreaterThanOrEqualExpression_Double_LeftEqualToRight_ShouldReturnTrue()
        {
            // Arrange
            var expression = new GreaterThanOrEqualExpression<double>
            {
                Left = 5.5,
                Right = 5.5
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GreaterThanOrEqualExpression_String_LeftGreaterThanRight_ShouldReturnTrue()
        {
            // Arrange
            var expression = new GreaterThanOrEqualExpression<string>
            {
                Left = "zebra",
                Right = "apple"
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GreaterThanOrEqualExpression_String_LeftEqualToRight_ShouldReturnTrue()
        {
            // Arrange
            var expression = new GreaterThanOrEqualExpression<string>
            {
                Left = "apple",
                Right = "apple"
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GreaterThanOrEqualExpression_String_LeftLessThanRight_ShouldReturnFalse()
        {
            // Arrange
            var expression = new GreaterThanOrEqualExpression<string>
            {
                Left = "apple",
                Right = "zebra"
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GreaterThanOrEqualExpression_WithDefaultValues_ShouldReturnTrue()
        {
            // Arrange
            var expression = new GreaterThanOrEqualExpression<int>();

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GreaterThanOrEqualExpression_WithOnlyLeftSet_ShouldReturnTrue()
        {
            // Arrange
            var expression = new GreaterThanOrEqualExpression<int>
            {
                Left = 5
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GreaterThanOrEqualExpression_WithOnlyRightSet_ShouldReturnFalse()
        {
            // Arrange
            var expression = new GreaterThanOrEqualExpression<int>
            {
                Right = 5
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GreaterThanOrEqualExpression_WithNegativeNumbers_ShouldWorkCorrectly()
        {
            // Arrange
            var expression = new GreaterThanOrEqualExpression<int>
            {
                Left = -3,
                Right = -5
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GreaterThanOrEqualExpression_WithEqualNegativeNumbers_ShouldReturnTrue()
        {
            // Arrange
            var expression = new GreaterThanOrEqualExpression<int>
            {
                Left = -5,
                Right = -5
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.True(result);
        }
    }
} 