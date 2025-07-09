using JstFlow.Attributes;
using JstFlow.Internal.Metas;
using System;

namespace JstFlow.External.Expressions
{
    [FlowExpr("除法")]
    public class DivideExpression<T> : FlowExpression<T> where T : struct
    {
        [Input("左操作数")]
        public T Left { get; set; }

        [Input("右操作数")]
        public T Right { get; set; }

        public override T Evaluate()
        {
            if (Right.Equals(default(T)))
            {
                throw new DivideByZeroException("除数不能为零");
            }
            return (T)((dynamic)Left / (dynamic)Right);
        }
    }
} 