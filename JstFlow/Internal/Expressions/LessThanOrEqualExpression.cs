using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JstFlow.Common;
using JstFlow.Interface;
using JstFlow.Internal.Base;
using JstFlow.Internal.ValueTypes;

namespace JstFlow.Internal.Expressions
{
    public class LessThanOrEqualExpression : IExpression
    {
        private static readonly Label _leftOperandLabel = new Label("left", "左操作数");
        private static readonly Label _rightOperandLabel = new Label("right", "右操作数");

        public string ExpressionName => "小于等于比较";

        public string ExpressionCode => "<=";

        public Label<IValueType> ReturnType => new Label<IValueType>("result", "比较结果", BoolType.Instance);

        public IDictionary<Label, IParameter> Inputs => new Dictionary<Label, IParameter>
        {
            { _leftOperandLabel, new Parameter(NumberType.Instance, true) },
            { _rightOperandLabel, new Parameter(NumberType.Instance, true) }
        };

        public async Task<Res<Label<IValue>>> Evaluate(IDictionary<Label, IValue> inputs)
        {
            try
            {
                // 验证输入参数
                if (inputs == null)
                {
                    return Res<Label<IValue>>.Fail("输入参数不能为空");
                }

                if (!inputs.TryGetValue(_leftOperandLabel, out var leftValue))
                {
                    return Res<Label<IValue>>.Fail($"缺少必需的输入参数: {_leftOperandLabel.Name}");
                }

                if (!inputs.TryGetValue(_rightOperandLabel, out var rightValue))
                {
                    return Res<Label<IValue>>.Fail($"缺少必需的输入参数: {_rightOperandLabel.Name}");
                }

                // 验证输入类型
                if (!(leftValue is NumberValue) || !(rightValue is NumberValue))
                {
                    return Res<Label<IValue>>.Fail("小于等于比较只支持数字类型");
                }

                // 执行小于等于比较
                var leftNumber = leftValue.GetValue<decimal>();
                var rightNumber = rightValue.GetValue<decimal>();
                bool isLessOrEqual = leftNumber <= rightNumber;

                // 创建结果
                var resultValue = new BoolValue(isLessOrEqual);
                var result = new Label<IValue>("result", "比较结果", resultValue);

                return Res<Label<IValue>>.Ok(result, "小于等于比较执行成功");
            }
            catch (Exception ex)
            {
                return Res<Label<IValue>>.Fail($"小于等于比较执行失败: {ex.Message}");
            }
        }
    }
} 