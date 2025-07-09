using System;
using Xunit;
using JstFlow.External.Expressions;

namespace Test.JstFlow.Node.Expressions
{
    public class ArithmeticExpressionTests
    {
        [Fact]
        public void AddExpression_ShouldAddIntegers()
        {
            var expr = new AddExpression<int> { Left = 5, Right = 3 };
            Assert.Equal(8, expr.Evaluate());
        }

        [Fact]
        public void AddExpression_ShouldAddDoubles()
        {
            var expr = new AddExpression<double> { Left = 5.5, Right = 3.2 };
            Assert.Equal(8.7, expr.Evaluate(), 2);
        }

        [Fact]
        public void SubtractExpression_ShouldSubtractIntegers()
        {
            var expr = new SubtractExpression<int> { Left = 10, Right = 3 };
            Assert.Equal(7, expr.Evaluate());
        }

        [Fact]
        public void SubtractExpression_ShouldSubtractDoubles()
        {
            var expr = new SubtractExpression<double> { Left = 10.5, Right = 3.2 };
            Assert.Equal(7.3, expr.Evaluate(), 2);
        }

        [Fact]
        public void MultiplyExpression_ShouldMultiplyIntegers()
        {
            var expr = new MultiplyExpression<int> { Left = 5, Right = 3 };
            Assert.Equal(15, expr.Evaluate());
        }

        [Fact]
        public void MultiplyExpression_ShouldMultiplyDoubles()
        {
            var expr = new MultiplyExpression<double> { Left = 5.5, Right = 3.0 };
            Assert.Equal(16.5, expr.Evaluate(), 2);
        }

        [Fact]
        public void DivideExpression_ShouldDivideIntegers()
        {
            var expr = new DivideExpression<int> { Left = 15, Right = 3 };
            Assert.Equal(5, expr.Evaluate());
        }

        [Fact]
        public void DivideExpression_ShouldDivideDoubles()
        {
            var expr = new DivideExpression<double> { Left = 15.0, Right = 3.0 };
            Assert.Equal(5.0, expr.Evaluate(), 2);
        }

        [Fact]
        public void DivideExpression_ShouldThrowException_WhenDividingByZero()
        {
            var expr = new DivideExpression<int> { Left = 10, Right = 0 };
            Assert.Throws<DivideByZeroException>(() => expr.Evaluate());
        }

        [Fact]
        public void ArithmeticExpressions_ShouldWorkWithDifferentNumericTypes()
        {
            // Test with different numeric types
            var intAdd = new AddExpression<int> { Left = 5, Right = 3 };
            Assert.Equal(8, intAdd.Evaluate());

            var doubleAdd = new AddExpression<double> { Left = 5.5, Right = 3.2 };
            Assert.Equal(8.7, doubleAdd.Evaluate(), 2);

            var floatAdd = new AddExpression<float> { Left = 5.5f, Right = 3.2f };
            Assert.Equal(8.7f, floatAdd.Evaluate(), 2);
        }
    }
} 