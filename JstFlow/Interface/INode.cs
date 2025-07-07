using JstFlow.Common;
using JstFlow.Interface.Models;
using JstFlow.Internal.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JstFlow.Interface
{
    /// <summary>
    /// 节点
    /// </summary>
    public interface INode
    {

        /// <summary>
        /// 节点名称
        /// </summary>
        string NodeName { get; }

        string NodeCode { get; }



        
        /// <summary>
        /// 输入 定义
        /// </summary>
        IDictionary<Label, IParameter> Inputs { get; }

        /// <summary>
        /// 输出 定义
        /// </summary>
        IDictionary<Label, IValue> Outputs { get; }

        
        /// <summary>
        /// 输出动作
        /// </summary>
        IList<Label> OutputActions { get; }

        /// <summary>
        /// 校验参数是否合法
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        Res IsValid(IDictionary<Label, IValue> inputs);




        /// <summary>
        /// 执行
        /// </summary>
        Task<Res<NodeExecuteResult>> Execute(IDictionary<Label, IValue> inputs);





    }
}
