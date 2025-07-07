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
    public class EqualExpression : IExpression
    {
        private static readonly Label _leftOperandLabel = new Label("left", "左操作数");
        private static readonly Label _rightOperandLabel = new Label("right", "右操作数");
        private static readonly Label _resultLabel = new Label("result", "比较结果");

        public string ExpressionName => "相等比较";

        public string ExpressionCode => "==";

        public Label<IValueType> ReturnType => new Label<IValueType>("result", "比较结果", null);

        public IDictionary<Label, IParameter> Inputs => new Dictionary<Label, IParameter>
        {
            { _leftOperandLabel, new Parameter(AnyType.Instance, true) },
            { _rightOperandLabel, new Parameter(AnyType.Instance, true) }
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

                // 执行相等比较
                bool isEqual = AreValuesEqual(leftValue, rightValue);

                // 创建结果
                var resultValue = new BoolValue(isEqual);
                var result = new Label<IValue>("result", "比较结果", resultValue);

                return Res<Label<IValue>>.Ok(result, "相等比较执行成功");
            }
            catch (Exception ex)
            {
                return Res<Label<IValue>>.Fail($"相等比较执行失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 比较两个值是否相等
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="right">右操作数</param>
        /// <returns>如果两个值相等则返回true</returns>
        private bool AreValuesEqual(IValue left, IValue right)
        {
            // 如果两个值都是null，则相等
            if (left == null && right == null)
                return true;

            // 如果只有一个值是null，则不相等
            if (left == null || right == null)
                return false;

            // 如果两个值类型相同，使用类型的Equals方法
            if (left.ValueType.Equals(right.ValueType))
            {
                return left.Equals(right);
            }

            // 如果类型不同，尝试转换为相同类型进行比较
            try
            {
                // 尝试将两个值都转换为字符串进行比较
                var leftString = left.ToString();
                var rightString = right.ToString();
                return leftString == rightString;
            }
            catch
            {
                // 如果转换失败，则认为不相等
                return false;
            }
        }
    }
}
