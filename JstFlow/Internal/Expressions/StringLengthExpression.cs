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
    public class StringLengthExpression : IExpression
    {
        private static readonly Label _inputLabel = new Label("input", "输入字符串");

        public string ExpressionName => "字符串长度";

        public string ExpressionCode => "string.length";

        public Label<IValueType> ReturnType => new Label<IValueType>("result", "字符串长度", NumberType.Instance);

        public IDictionary<Label, IParameter> Inputs => new Dictionary<Label, IParameter>
        {
            { _inputLabel, new Parameter(StringType.Instance, true) }
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

                if (!inputs.TryGetValue(_inputLabel, out var inputValue))
                {
                    return Res<Label<IValue>>.Fail($"缺少必需的输入参数: {_inputLabel.Name}");
                }

                // 验证输入类型
                if (!(inputValue is StringValue))
                {
                    return Res<Label<IValue>>.Fail("字符串长度计算只支持字符串类型");
                }

                // 执行字符串长度计算
                var inputString = inputValue.GetValue<string>();
                int length = inputString?.Length ?? 0;

                // 创建结果
                var resultValue = new NumberValue(length);
                var result = new Label<IValue>("result", "字符串长度", resultValue);

                return Res<Label<IValue>>.Ok(result, "字符串长度计算执行成功");
            }
            catch (Exception ex)
            {
                return Res<Label<IValue>>.Fail($"字符串长度计算执行失败: {ex.Message}");
            }
        }
    }
} 