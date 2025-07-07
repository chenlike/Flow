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

    // 节点基础信息
    string NodeName { get; }
    string NodeCode { get; }
    IDictionary<Label, IParameter> Inputs { get; }
    IDictionary<Label, IValueType> Outputs { get; }
    IList<Label> OutputActions { get; }
    IList<Label> InputActions { get; }

    event Func<string, Task> OnNodeEvent;
    event Action OnCompleted;
    // 启动节点
    Task StartAsync(IDictionary<Label, IValue> inputs);

    }
}
