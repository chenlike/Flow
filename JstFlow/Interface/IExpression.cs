using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JstFlow.Common;
using JstFlow.Internal.Base;

namespace JstFlow.Interface
{
    public interface IExpression
    {
        string ExpressionName { get; }
        /// <summary>
        /// 表达式标识符（可选）
        /// </summary>
        string ExpressionCode { get; }

        /// <summary>
        /// 表达式期望返回的类型
        /// </summary>
        Label<IValue> ReturnType { get; }

        /// <summary>
        /// 输入 定义
        /// </summary>
        IDictionary<Label, IParameter> Inputs { get; }

        /// <summary>
        /// 执行表达式
        /// </summary>
        /// <param name="inputs">输入的变量值，例如来自节点上下文</param>
        /// <returns>表达式计算结果</returns>
        Task<Res<IDictionary<Label, IValue>>> Evaluate(IDictionary<Label, IValue> inputs);
    }
}
