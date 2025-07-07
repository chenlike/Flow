using JstFlow.Common;
using JstFlow.Interface;
using JstFlow.Interface.Models;
using JstFlow.Internal.Base;
using JstFlow.Internal.ValueTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JstFlow.Internal.Nodes
{
    public class IfNode : INode
    {
        public string NodeName => "判断分支";

        public string NodeCode => "if";

        public IDictionary<Label, IParameter> Inputs => new Dictionary<Label, IParameter>()
        {
            { new Label("conditionA","条件A"), new Parameter(AnyType.Instance, true) },
            { new Label("conditionB","条件B"), new Parameter(AnyType.Instance, true) },
        };

        public IDictionary<Label, IValueType> Outputs => new Dictionary<Label, IValueType>()
        {
            { new Label("result","结果"), BoolType.Instance }
        };

        public IList<Label> OutputActions => new List<Label>()
        {
            new Label("true","真"),
            new Label("false","假")
        };

        public Task<Res<NodeExecuteResult>> Execute(IDictionary<Label, IValue> inputs)
        {
            var conditionA = inputs["conditionA"];
            var conditionB = inputs["conditionB"];
            if (conditionA.Equals(conditionB))
            {
                return Task.FromResult(Res<NodeExecuteResult>.Ok(new NodeExecuteResult(){
                    Outputs = new Dictionary<Label, IValue>()
                    {
                        { new Label("result","结果"), new BoolValue(true) }
                    },
                    ActionToExecute = new Label("true","真")
                }));
            }else{
                return Task.FromResult(Res<NodeExecuteResult>.Ok(new NodeExecuteResult(){
                    Outputs = new Dictionary<Label, IValue>()
                    {
                        { new Label("result","结果"), new BoolValue(false) }
                    },
                    ActionToExecute = new Label("false","假")
                }));
            }
        }

        public Res IsValid(IDictionary<Label, IValue> inputs)
        {
            // 判断输入的2个参数 是否类型一致
            if (inputs.Count != 2)
            {
                return Res.Fail("输入参数数量不正确");
            }
            var conditionA = inputs["conditionA"];
            var conditionB = inputs["conditionB"];

            // 判断类型是否一致  根据Type
            if (conditionA.ValueType != conditionB.ValueType)
            {
                return Res.Fail("输入参数类型不一致");
            }
            return Res.Ok();

        }
    }
}
