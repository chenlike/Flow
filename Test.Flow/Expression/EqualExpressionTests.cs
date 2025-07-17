using Flow.External.Expressions;
using Xunit;
using System;

namespace Test.Flow.Expression
{
    public class EqualExpressionTests
    {
        [Fact]
        public void EqualExpression_Int_EqualValues_ShouldReturnTrue()
        {
            // Arrange
            var expression = new EqualExpression<int>
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
        public void EqualExpression_Int_DifferentValues_ShouldReturnFalse()
        {
            // Arrange
            var expression = new EqualExpression<int>
            {
                Left = 5,
                Right = 3
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void EqualExpression_String_EqualValues_ShouldReturnTrue()
        {
            // Arrange
            var expression = new EqualExpression<string>
            {
                Left = "hello",
                Right = "hello"
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void EqualExpression_String_DifferentValues_ShouldReturnFalse()
        {
            // Arrange
            var expression = new EqualExpression<string>
            {
                Left = "hello",
                Right = "world"
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void EqualExpression_Double_EqualValues_ShouldReturnTrue()
        {
            // Arrange
            var expression = new EqualExpression<double>
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
        public void EqualExpression_Double_DifferentValues_ShouldReturnFalse()
        {
            // Arrange
            var expression = new EqualExpression<double>
            {
                Left = 5.5,
                Right = 5.6
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void EqualExpression_WithDefaultValues_ShouldReturnTrue()
        {
            // Arrange
            var expression = new EqualExpression<int>();

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void EqualExpression_WithOnlyLeftSet_ShouldReturnFalse()
        {
            // Arrange
            var expression = new EqualExpression<int>
            {
                Left = 5
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void EqualExpression_WithOnlyRightSet_ShouldReturnFalse()
        {
            // Arrange
            var expression = new EqualExpression<int>
            {
                Right = 5
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void EqualExpression_String_WithNullValues_ShouldReturnTrue()
        {
            // Arrange
            var expression = new EqualExpression<string>
            {
                Left = null,
                Right = null
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void EqualExpression_String_WithOneNullValue_ShouldReturnFalse()
        {
            // Arrange
            var expression = new EqualExpression<string>
            {
                Left = "hello",
                Right = null
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.False(result);
        }
    }
} 