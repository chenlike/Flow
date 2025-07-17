using Flow.Attributes;
using Flow.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Flow.External.Nodes
{

    public class FlowOutEvent
    {
        public string MemberName { get; }

        private FlowOutEvent(string memberName)
        {
            MemberName = memberName;
        }

        public static FlowOutEvent Of(Expression<Func<FlowEndpoint>> expression)
        {
            var memberName = GetMemberName(expression);
            if (memberName == null)
            {
                throw new ArgumentException("无效的表达式，无法提取成员名称");
            }

            return new FlowOutEvent(memberName);
        }

        private static string GetMemberName(Expression expression)
        {
            if (expression is LambdaExpression lambda &&
                lambda.Body is MemberExpression memberExpr)
            {
                return memberExpr.Member.Name;
            }
            return null;
        }
    }


    public abstract class FlowBaseNode
    {

        public FlowOutEvent Emit(Expression<Func<FlowEndpoint>> expression)
        {
            return FlowOutEvent.Of(expression);
        }


    }
}
