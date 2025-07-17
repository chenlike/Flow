using Flow.External.Expressions;
using Xunit;
using System;

namespace Test.Flow.Expression
{
    public class LessThanOrEqualExpressionTests
    {
        [Fact]
        public void LessThanOrEqualExpression_Int_LeftLessThanRight_ShouldReturnTrue()
        {
            // Arrange
            var expression = new LessThanOrEqualExpression<int>
            {
                Left = 3,
                Right = 5
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void LessThanOrEqualExpression_Int_LeftEqualToRight_ShouldReturnTrue()
        {
            // Arrange
            var expression = new LessThanOrEqualExpression<int>
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
        public void LessThanOrEqualExpression_Int_LeftGreaterThanRight_ShouldReturnFalse()
        {
            // Arrange
            var expression = new LessThanOrEqualExpression<int>
            {
                Left = 10,
                Right = 5
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void LessThanOrEqualExpression_Double_LeftLessThanRight_ShouldReturnTrue()
        {
            // Arrange
            var expression = new LessThanOrEqualExpression<double>
            {
                Left = 3.2,
                Right = 5.5
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void LessThanOrEqualExpression_Double_LeftEqualToRight_ShouldReturnTrue()
        {
            // Arrange
            var expression = new LessThanOrEqualExpression<double>
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
        public void LessThanOrEqualExpression_String_LeftLessThanRight_ShouldReturnTrue()
        {
            // Arrange
            var expression = new LessThanOrEqualExpression<string>
            {
                Left = "apple",
                Right = "zebra"
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void LessThanOrEqualExpression_String_LeftEqualToRight_ShouldReturnTrue()
        {
            // Arrange
            var expression = new LessThanOrEqualExpression<string>
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
        public void LessThanOrEqualExpression_String_LeftGreaterThanRight_ShouldReturnFalse()
        {
            // Arrange
            var expression = new LessThanOrEqualExpression<string>
            {
                Left = "zebra",
                Right = "apple"
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void LessThanOrEqualExpression_WithDefaultValues_ShouldReturnTrue()
        {
            // Arrange
            var expression = new LessThanOrEqualExpression<int>();

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void LessThanOrEqualExpression_WithOnlyLeftSet_ShouldReturnFalse()
        {
            // Arrange
            var expression = new LessThanOrEqualExpression<int>
            {
                Left = 5
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void LessThanOrEqualExpression_WithOnlyRightSet_ShouldReturnTrue()
        {
            // Arrange
            var expression = new LessThanOrEqualExpression<int>
            {
                Right = 5
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void LessThanOrEqualExpression_WithNegativeNumbers_ShouldWorkCorrectly()
        {
            // Arrange
            var expression = new LessThanOrEqualExpression<int>
            {
                Left = -5,
                Right = -3
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void LessThanOrEqualExpression_WithEqualNegativeNumbers_ShouldReturnTrue()
        {
            // Arrange
            var expression = new LessThanOrEqualExpression<int>
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