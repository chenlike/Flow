using JstFlow.Attributes;
using JstFlow.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace JstFlow.External.Nodes
{

    public class FlowOutEvent
    {
        public Expression<Func<FlowEndpoint>> Expression { get; }

        private FlowOutEvent(Expression<Func<FlowEndpoint>> expression)
        {
            Expression = expression;
        }

        public static FlowOutEvent Of(Expression<Func<FlowEndpoint>> expression)
        {
            return new FlowOutEvent(expression);
        }

        public FlowEndpoint Invoke() => Expression.Compile().Invoke();

        public FlowEventAttribute GetLabelAttribute()
        {
            var memberInfo = GetMemberInfo(Expression);
            return memberInfo?.GetCustomAttribute<FlowEventAttribute>();
        }

        private static MemberInfo GetMemberInfo(Expression expression)
        {
            if (expression is LambdaExpression lambda &&
                lambda.Body is MemberExpression memberExpr)
                return memberExpr.Member;
            return null;
        }
    }


    public abstract class FlowBaseNode
    {

        internal NodeContext _context;


#if DEBUG
        public void Inject(NodeContext context)
#endif
#if RELEASE
        internal void Inject(NodeContext context)
#endif
        {
            _context = context;
        }

        public FlowOutEvent MoveNext(Expression<Func<FlowEndpoint>> expression)
        {
            return FlowOutEvent.Of(expression);
        }
        protected void Execute(Expression<Func<FlowEndpoint>> expression)
        {
            _context.TriggerFlowEvent(FlowOutEvent.Of(expression));
        }



    }
}
