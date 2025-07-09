using System;
using JstFlow.External.Expressions;

namespace JstFlow
{
    public class TestEqualExpression
    {
        public static void Test()
        {
            Console.WriteLine("测试 EqualExpression...");
            
            // 测试整数相等
            var intExpr = new EqualExpression<int> { Left = 5, Right = 5 };
            Console.WriteLine($"5 == 5: {intExpr.Evaluate()}");
            
            var intExpr2 = new EqualExpression<int> { Left = 5, Right = 10 };
            Console.WriteLine($"5 == 10: {intExpr2.Evaluate()}");
            
            // 测试字符串相等
            var stringExpr = new EqualExpression<string> { Left = "hello", Right = "hello" };
            Console.WriteLine($"'hello' == 'hello': {stringExpr.Evaluate()}");
            
            var stringExpr2 = new EqualExpression<string> { Left = "hello", Right = "world" };
            Console.WriteLine($"'hello' == 'world': {stringExpr2.Evaluate()}");
            
            Console.WriteLine("测试完成！");
        }
    }
} 