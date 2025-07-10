using System;
using Xunit;
using JstFlow.External.Expressions;

namespace Test.JstFlow.Node.Expressions
{
    public class ConcatExpressionTests
    {
        [Fact]
        public void ConcatExpression_ShouldConcatenateAllStrings()
        {
            var expr = new ConcatExpression
            {
                String1 = "Hello",
                String2 = " ",
                String3 = "World",
                String4 = "!",
                String5 = " Test"
            };
            
            Assert.Equal("Hello World! Test", expr.Evaluate());
        }

        [Fact]
        public void ConcatExpression_ShouldHandleNullValues()
        {
            var expr = new ConcatExpression
            {
                String1 = "Hello",
                String2 = null,
                String3 = "World",
                String4 = null,
                String5 = "!"
            };
            
            Assert.Equal("HelloWorld!", expr.Evaluate());
        }

        [Fact]
        public void ConcatExpression_ShouldHandleEmptyStrings()
        {
            var expr = new ConcatExpression
            {
                String1 = "",
                String2 = "Hello",
                String3 = "",
                String4 = "World",
                String5 = ""
            };
            
            Assert.Equal("HelloWorld", expr.Evaluate());
        }

        [Fact]
        public void ConcatExpression_ShouldHandleAllNullValues()
        {
            var expr = new ConcatExpression
            {
                String1 = null,
                String2 = null,
                String3 = null,
                String4 = null,
                String5 = null
            };
            
            Assert.Equal("", expr.Evaluate());
        }

        [Fact]
        public void ConcatExpression_ShouldHandleMixedNullAndEmptyStrings()
        {
            var expr = new ConcatExpression
            {
                String1 = null,
                String2 = "",
                String3 = "Hello",
                String4 = null,
                String5 = "World"
            };
            
            Assert.Equal("HelloWorld", expr.Evaluate());
        }

        [Fact]
        public void ConcatExpression_ShouldHandleSpecialCharacters()
        {
            var expr = new ConcatExpression
            {
                String1 = "测试",
                String2 = "中文",
                String3 = "123",
                String4 = "!@#",
                String5 = "符号"
            };
            
            Assert.Equal("测试中文123!@#符号", expr.Evaluate());
        }

        [Fact]
        public void ConcatExpression_ShouldHandleUnicodeCharacters()
        {
            var expr = new ConcatExpression
            {
                String1 = "Hello",
                String2 = "世界",
                String3 = "🌍",
                String4 = "Test",
                String5 = "测试"
            };
            
            Assert.Equal("Hello世界🌍Test测试", expr.Evaluate());
        }

        [Fact]
        public void ConcatExpression_ShouldHandleSingleString()
        {
            var expr = new ConcatExpression
            {
                String1 = "Single",
                String2 = null,
                String3 = null,
                String4 = null,
                String5 = null
            };
            
            Assert.Equal("Single", expr.Evaluate());
        }

        [Fact]
        public void ConcatExpression_ShouldHandleNumbersAsStrings()
        {
            var expr = new ConcatExpression
            {
                String1 = "1",
                String2 = "2",
                String3 = "3",
                String4 = "4",
                String5 = "5"
            };
            
            Assert.Equal("12345", expr.Evaluate());
        }
    }
} 