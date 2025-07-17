using Flow.External.Expressions;
using Xunit;
using System;
using System.Collections.Generic;

namespace Test.Flow.Expression
{
    public class ConcatExpressionTests
    {
        [Fact]
        public void ConcatExpression_WithMultipleStrings_ShouldConcatenateCorrectly()
        {
            // Arrange
            var expression = new ConcatExpression
            {
                StringList = new List<string> { "Hello", " ", "World", "!" }
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal("Hello World!", result);
        }

        [Fact]
        public void ConcatExpression_WithEmptyList_ShouldReturnEmptyString()
        {
            // Arrange
            var expression = new ConcatExpression
            {
                StringList = new List<string>()
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void ConcatExpression_WithNullList_ShouldReturnEmptyString()
        {
            // Arrange
            var expression = new ConcatExpression
            {
                StringList = null
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void ConcatExpression_WithSingleString_ShouldReturnThatString()
        {
            // Arrange
            var expression = new ConcatExpression
            {
                StringList = new List<string> { "Hello" }
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal("Hello", result);
        }

        [Fact]
        public void ConcatExpression_WithEmptyStrings_ShouldConcatenateCorrectly()
        {
            // Arrange
            var expression = new ConcatExpression
            {
                StringList = new List<string> { "", "Hello", "", "World", "" }
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal("HelloWorld", result);
        }

        [Fact]
        public void ConcatExpression_WithAllEmptyStrings_ShouldReturnEmptyString()
        {
            // Arrange
            var expression = new ConcatExpression
            {
                StringList = new List<string> { "", "", "" }
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void ConcatExpression_WithNullStrings_ShouldHandleCorrectly()
        {
            // Arrange
            var expression = new ConcatExpression
            {
                StringList = new List<string> { "Hello", null, "World" }
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal("HelloWorld", result);
        }

        [Fact]
        public void ConcatExpression_WithNumbersAsStrings_ShouldConcatenateCorrectly()
        {
            // Arrange
            var expression = new ConcatExpression
            {
                StringList = new List<string> { "1", "2", "3", "4", "5" }
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal("12345", result);
        }

        [Fact]
        public void ConcatExpression_WithSpecialCharacters_ShouldConcatenateCorrectly()
        {
            // Arrange
            var expression = new ConcatExpression
            {
                StringList = new List<string> { "Hello", "世界", "!", "123", "测试" }
            };

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal("Hello世界!123测试", result);
        }

        [Fact]
        public void ConcatExpression_WithDefaultValue_ShouldReturnEmptyString()
        {
            // Arrange
            var expression = new ConcatExpression();

            // Act
            var result = expression.Evaluate();

            // Assert
            Assert.Equal("", result);
        }
    }
} 