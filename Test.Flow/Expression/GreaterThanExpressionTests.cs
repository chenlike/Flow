using Flow.External.Expressions;
using Xunit;
using System;

namespace Test.Flow.Expression
{
    public class GreaterThanExpressionTests
    {
        [Fact]
        public void GreaterThanExpression_Int_LeftGreaterThanRight_ShouldReturnTrue()
        {
            // Arrange
            var expression = new GreaterThanExpression<int>
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
        public void GreaterThanExpression_Int_LeftEqualToRight_ShouldReturnFalse()
        {
            // Arrange
            var expression = new GreaterThanExpression<int>
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
        public void GreaterThanExpression_Int_LeftLessThanRight_ShouldReturnFalse()
        {
            // Arrange
            var expression = new GreaterThanExpression<int>
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
        public void GreaterThanExpression_Double_LeftGreaterThanRight_ShouldReturnTrue()
        {
            // Arrange
            var expression = new GreaterThanExpression<double>
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
        public void GreaterThanExpression_Double_LeftEqualToRight_ShouldReturnFalse()
        {
            // Arrange
            var expression = new GreaterThanExpression<double>
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
        public void GreaterThanExpression_String_LeftGreaterThanRight_ShouldReturnTrue()
        {
            // Arrange
            var expression = new GreaterThanExpression<string>
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
        public void GreaterThanExpression_String_LeftEqualToRight_ShouldReturnFalse()
        {
            // Arrange
            var expression = new GreaterThanExpression<string>
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
        public void GreaterThanExpression_String_LeftLessThanRight_ShouldReturnFalse()
        {
            // Arrange
            var expression = new GreaterThanExpression<string>
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
        public void GreaterThanExpression_WithDefaultValues_ShouldReturnFalse()
        {
            // Arrange
            var expression = new GreaterThanExpression<int>();

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GreaterThanExpression_WithOnlyLeftSet_ShouldReturnTrue()
        {
            // Arrange
            var expression = new GreaterThanExpression<int>
            {
                Left = 5
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GreaterThanExpression_WithOnlyRightSet_ShouldReturnFalse()
        {
            // Arrange
            var expression = new GreaterThanExpression<int>
            {
                Right = 5
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GreaterThanExpression_WithNegativeNumbers_ShouldWorkCorrectly()
        {
            // Arrange
            var expression = new GreaterThanExpression<int>
            {
                Left = -3,
                Right = -5
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.True(result);
        }
    }
} 