using System;
using Xunit;
using JstFlow.External.Expressions;

namespace Test.JstFlow.Node.Expressions
{
    public class EqualExpressionTests
    {
        [Fact]
        public void IntEqualExpression_ShouldReturnTrue_WhenValuesAreEqual()
        {
            // Arrange
            var expr = new EqualExpression<int> { Left = 123, Right = 123 };
            
            // Act
            var result = expr.Evaluate();
            
            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IntEqualExpression_ShouldReturnFalse_WhenValuesAreNotEqual()
        {
            // Arrange
            var expr = new EqualExpression<int> { Left = 123, Right = 456 };
            
            // Act
            var result = expr.Evaluate();
            
            // Assert
            Assert.False(result);
        }

        [Fact]
        public void StringEqualExpression_ShouldReturnTrue_WhenValuesAreEqual()
        {
            // Arrange
            var expr = new EqualExpression<string> { Left = "abc", Right = "abc" };
            
            // Act
            var result = expr.Evaluate();
            
            // Assert
            Assert.True(result);
        }

        [Fact]
        public void StringEqualExpression_ShouldReturnFalse_WhenValuesAreNotEqual()
        {
            // Arrange
            var expr = new EqualExpression<string> { Left = "abc", Right = "def" };
            
            // Act
            var result = expr.Evaluate();
            
            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CustomTypeEqualExpression_ShouldWorkWithIEquatable()
        {
            // Arrange
            var expr = new EqualExpression<MyEquatable> { Left = new MyEquatable(1), Right = new MyEquatable(1) };
            
            // Act & Assert
            Assert.True(expr.Evaluate());
            
            expr.Right = new MyEquatable(2);
            Assert.False(expr.Evaluate());
        }

        private class MyEquatable : IEquatable<MyEquatable>
        {
            public int Value { get; }
            public MyEquatable(int value) => Value = value;
            public bool Equals(MyEquatable other) => other != null && Value == other.Value;
            public override bool Equals(object obj) => obj is MyEquatable other && Equals(other);
            public override int GetHashCode() => Value.GetHashCode();
        }
    }
} 