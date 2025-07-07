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
    public class StringIsEmptyExpression : IExpression
    {
        private static readonly Label _inputLabel = new Label("input", "输入字符串");

        public string ExpressionName => "字符串是否为空";

        public string ExpressionCode => "string.isEmpty";

        public Label<IValueType> ReturnType => new Label<IValueType>("result", "判断结果", BoolType.Instance);

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
                    return Res<Label<IValue>>.Fail("字符串是否为空判断只支持字符串类型");
                }

                // 执行字符串是否为空判断
                var inputString = inputValue.GetValue<string>();
                bool isEmpty = string.IsNullOrEmpty(inputString);

                // 创建结果
                var resultValue = new BoolValue(isEmpty);
                var result = new Label<IValue>("result", "判断结果", resultValue);

                return Res<Label<IValue>>.Ok(result, "字符串是否为空判断执行成功");
            }
            catch (Exception ex)
            {
                return Res<Label<IValue>>.Fail($"字符串是否为空判断执行失败: {ex.Message}");
            }
        }
    }
} 