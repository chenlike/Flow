using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JstFlow.Common;
using JstFlow.Interface;
using JstFlow.Interface.Models;
using JstFlow.Internal.Base;
using JstFlow.Internal.ValueTypes;

namespace JstFlow.Internal.Nodes
{
    public class ForNode : INode
    {
        private static readonly Label _startLabel = new Label("start", "起始值");
        private static readonly Label _endLabel = new Label("end", "结束值");
        private static readonly Label _stepLabel = new Label("step", "步长");
        private static readonly Label _currentIndexLabel = new Label("currentIndex", "当前索引");
        private static readonly Label _isFirstLabel = new Label("isFirst", "是否第一次");
        private static readonly Label _isLastLabel = new Label("isLast", "是否最后一次");

        // 事件
        public event Func<string, Task> OnNodeEvent;
        public event Action OnCompleted;

        // 状态
        private bool _isRunning = false;
        private decimal _currentIndex = 0;
        private decimal _start = 0;
        private decimal _end = 0;
        private decimal _step = 1;
        private bool _shouldBreak = false;

        public string NodeName => "For循环";

        public string NodeCode => "for";

        public IDictionary<Label, IParameter> Inputs => new Dictionary<Label, IParameter>
        {
            { _startLabel, new Parameter(NumberType.Instance, true) },
            { _endLabel, new Parameter(NumberType.Instance, true) },
            { _stepLabel, new Parameter(NumberType.Instance, false) } // 步长可选，默认为1
        };

        public IDictionary<Label, IValueType> Outputs => new Dictionary<Label, IValueType>
        {
            { _currentIndexLabel, NumberType.Instance },
            { _isFirstLabel, BoolType.Instance },
            { _isLastLabel, BoolType.Instance }
        };

        public IList<Label> OutputActions => new List<Label>
        {
            new Label("loopAction", "循环动作")
        };

        public IList<Label> InputActions => new List<Label>
        {
            new Label("break", "中断循环"),
            new Label("continue", "跳过当前循环")
        };

        public async Task StartAsync(IDictionary<Label, IValue> inputs)
        {
            if (_isRunning)
            {
                throw new InvalidOperationException("For循环已在运行中");
            }

            try
            {
                // 验证并获取输入参数
                if (inputs == null)
                {
                    throw new ArgumentException("输入参数不能为空");
                }

                if (!inputs.TryGetValue(_startLabel, out var startValue))
                {
                    throw new ArgumentException($"缺少必需的输入参数: {_startLabel.Name}");
                }

                if (!inputs.TryGetValue(_endLabel, out var endValue))
                {
                    throw new ArgumentException($"缺少必需的输入参数: {_endLabel.Name}");
                }

                if (!inputs.TryGetValue(_stepLabel, out var stepValue))
                {
                    stepValue = new NumberValue(1); // 默认步长为1
                }

                // 验证输入类型
                if (!(startValue is NumberValue) || !(endValue is NumberValue) || !(stepValue is NumberValue))
                {
                    throw new ArgumentException("For循环只支持数字类型参数");
                }

                _start = startValue.GetValue<decimal>();
                _end = endValue.GetValue<decimal>();
                _step = stepValue.GetValue<decimal>();

                // 验证逻辑
                if (_start > _end)
                {
                    throw new ArgumentException("起始值不能大于结束值");
                }

                _isRunning = true;
                _shouldBreak = false;

                // 执行循环
                for (var i = _start; i <= _end && !_shouldBreak; i += _step)
                {
                    _currentIndex = i;

                    // 计算当前循环的状态
                    bool isFirst = i == _start;
                    bool isLast = i + _step > _end;

                    // 创建当前步骤的结果
                    var stepResult = new NodeExecuteResult
                    {
                        Outputs = new Dictionary<Label, IValue>
                        {
                            { _currentIndexLabel, new NumberValue(i) },
                            { _isFirstLabel, new BoolValue(isFirst) },
                            { _isLastLabel, new BoolValue(isLast) }
                        },
                        ActionToExecute = new Label("loopAction", "循环动作"),
                        Status = "running",
                        Signal = "normal",
                        ShouldContinue = true,
                        Message = $"执行第 {i} 次循环"
                    };

                    // 触发每步事件
                    if (OnNodeEvent != null)
                    {
                        await OnNodeEvent("step");
                    }

                    // 检查是否需要中断循环
                    if (OnNodeEvent != null)
                    {
                        await OnNodeEvent("break");
                        // 如果设置了break标志，退出循环
                        if (_shouldBreak)
                        {
                            break;
                        }
                    }

                    // 检查是否需要跳过当前循环
                    if (OnNodeEvent != null)
                    {
                        await OnNodeEvent("continue");
                    }

                    // 可以在这里添加延迟，避免过快执行
                    await Task.Delay(100);
                }

                // 循环完成，触发完成事件
                OnCompleted?.Invoke();
            }
            finally
            {
                _isRunning = false;
            }
        }

        /// <summary>
        /// 获取当前状态
        /// </summary>
        public (decimal currentIndex, bool isRunning) GetStatus()
        {
            return (_currentIndex, _isRunning);
        }

        /// <summary>
        /// 获取当前输出
        /// </summary>
        public IDictionary<Label, IValue> GetCurrentOutputs()
        {
            bool isFirst = _currentIndex == _start;
            bool isLast = _currentIndex + _step > _end;

            return new Dictionary<Label, IValue>
            {
                { _currentIndexLabel, new NumberValue(_currentIndex) },
                { _isFirstLabel, new BoolValue(isFirst) },
                { _isLastLabel, new BoolValue(isLast) }
            };
        }

        /// <summary>
        /// 设置break标志
        /// </summary>
        public void SetBreak()
        {
            _shouldBreak = true;
        }
    }
} 