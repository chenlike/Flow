using JstFlow.External.Expressions;
using Xunit;
using System;

namespace Test.JstFlow.Expression
{
    public class LessThanExpressionTests
    {
        [Fact]
        public void LessThanExpression_Int_LeftLessThanRight_ShouldReturnTrue()
        {
            // Arrange
            var expression = new LessThanExpression<int>
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
        public void LessThanExpression_Int_LeftEqualToRight_ShouldReturnFalse()
        {
            // Arrange
            var expression = new LessThanExpression<int>
            {
                Left = 5,
                Right = 5
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void LessThanExpression_Int_LeftGreaterThanRight_ShouldReturnFalse()
        {
            // Arrange
            var expression = new LessThanExpression<int>
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
        public void LessThanExpression_Double_LeftLessThanRight_ShouldReturnTrue()
        {
            // Arrange
            var expression = new LessThanExpression<double>
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
        public void LessThanExpression_Double_LeftEqualToRight_ShouldReturnFalse()
        {
            // Arrange
            var expression = new LessThanExpression<double>
            {
                Left = 5.5,
                Right = 5.5
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void LessThanExpression_String_LeftLessThanRight_ShouldReturnTrue()
        {
            // Arrange
            var expression = new LessThanExpression<string>
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
        public void LessThanExpression_String_LeftEqualToRight_ShouldReturnFalse()
        {
            // Arrange
            var expression = new LessThanExpression<string>
            {
                Left = "apple",
                Right = "apple"
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void LessThanExpression_String_LeftGreaterThanRight_ShouldReturnFalse()
        {
            // Arrange
            var expression = new LessThanExpression<string>
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
        public void LessThanExpression_WithDefaultValues_ShouldReturnFalse()
        {
            // Arrange
            var expression = new LessThanExpression<int>();

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void LessThanExpression_WithOnlyLeftSet_ShouldReturnFalse()
        {
            // Arrange
            var expression = new LessThanExpression<int>
            {
                Left = 5
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void LessThanExpression_WithOnlyRightSet_ShouldReturnTrue()
        {
            // Arrange
            var expression = new LessThanExpression<int>
            {
                Right = 5
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void LessThanExpression_WithNegativeNumbers_ShouldWorkCorrectly()
        {
            // Arrange
            var expression = new LessThanExpression<int>
            {
                Left = -5,
                Right = -3
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.True(result);
        }
    }
} 