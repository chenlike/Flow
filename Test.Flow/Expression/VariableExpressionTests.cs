using Flow.External.Expressions;
using Xunit;
using System;

namespace Test.Flow.Expression
{
    public class VariableExpressionTests
    {
        [Fact]
        public void VariableExpression_Int_ShouldReturnSetValue()
        {
            // Arrange
            var expression = new VariableExpression<int>
            {
                Value = 42
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(42, result);
        }

        [Fact]
        public void VariableExpression_String_ShouldReturnSetValue()
        {
            // Arrange
            var expression = new VariableExpression<string>
            {
                Value = "Hello World"
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal("Hello World", result);
        }

        [Fact]
        public void VariableExpression_Double_ShouldReturnSetValue()
        {
            // Arrange
            var expression = new VariableExpression<double>
            {
                Value = 3.14159
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(3.14159, result);
        }

        [Fact]
        public void VariableExpression_Bool_ShouldReturnSetValue()
        {
            // Arrange
            var expression = new VariableExpression<bool>
            {
                Value = true
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VariableExpression_WithDefaultValue_ShouldReturnDefault()
        {
            // Arrange
            var expression = new VariableExpression<int>();

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void VariableExpression_WithNullString_ShouldReturnNull()
        {
            // Arrange
            var expression = new VariableExpression<string>
            {
                Value = null
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void VariableExpression_WithEmptyString_ShouldReturnEmptyString()
        {
            // Arrange
            var expression = new VariableExpression<string>
            {
                Value = ""
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void VariableExpression_WithNegativeNumber_ShouldReturnNegativeValue()
        {
            // Arrange
            var expression = new VariableExpression<int>
            {
                Value = -42
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(-42, result);
        }

        [Fact]
        public void VariableExpression_WithZero_ShouldReturnZero()
        {
            // Arrange
            var expression = new VariableExpression<int>
            {
                Value = 0
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void VariableExpression_WithFalse_ShouldReturnFalse()
        {
            // Arrange
            var expression = new VariableExpression<bool>
            {
                Value = false
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void VariableExpression_WithCustomObject_ShouldReturnObject()
        {
            // Arrange
            var testObject = new { Name = "Test", Value = 123 };
            var expression = new VariableExpression<object>
            {
                Value = testObject
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(testObject, result);
        }

        [Fact]
        public void VariableExpression_WithLong_ShouldReturnLongValue()
        {
            // Arrange
            var expression = new VariableExpression<long>
            {
                Value = 1234567890123456789L
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal(1234567890123456789L, result);
        }
    }
} 