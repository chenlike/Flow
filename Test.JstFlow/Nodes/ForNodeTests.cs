using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JstFlow.Internal.Nodes;
using JstFlow.Internal.ValueTypes;
using JstFlow.Internal.Base;
using JstFlow.Interface;

namespace Test.JstFlow.Nodes
{
    public class ForNodeTests
    {
        [Fact]
        public void TestForNodeProperties()
        {
            var node = new ForNode();
            
            Assert.Equal("For循环", node.NodeName);
            Assert.Equal("for", node.NodeCode);
            Assert.NotNull(node.Inputs);
            Assert.NotNull(node.Outputs);
            Assert.NotNull(node.OutputActions);
            Assert.NotNull(node.InputActions);
            
            Assert.Equal(3, node.Inputs.Count); // start, end, step
            Assert.Equal(3, node.Outputs.Count); // currentIndex, isFirst, isLast
            Assert.Equal(1, node.OutputActions.Count); // loopAction
            Assert.Equal(2, node.InputActions.Count); // break, continue
        }

        [Fact]
        public async Task TestForNodeBasicExecution()
        {
            var node = new ForNode();
            var stepCount = 0;
            var completed = false;

            // 设置事件处理器
            node.OnNodeEvent += async (eventType) =>
            {
                if (eventType == "step")
                {
                    stepCount++;
                }
            };

            node.OnCompleted += () => completed = true;

            // 准备输入参数
            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("start", "起始值"), new NumberValue(1) as IValue },
                { new Label("end", "结束值"), new NumberValue(5) as IValue },
                { new Label("step", "步长"), new NumberValue(1) as IValue }
            };

            // 启动循环
            await node.StartAsync(inputs);

            // 验证结果
            Assert.Equal(5, stepCount); // 应该执行5次循环
            Assert.True(completed);
            
            var status = node.GetStatus();
            Assert.Equal(5, status.currentIndex);
            Assert.False(status.isRunning);
        }

        [Fact]
        public async Task TestForNodeWithBreak()
        {
            var node = new ForNode();
            var stepCount = 0;
            var completed = false;
            var shouldBreak = false;

            // 设置事件处理器
            node.OnNodeEvent += async (eventType) =>
            {
                if (eventType == "step")
                {
                    stepCount++;
                    // 在第3次循环时设置break标志
                    if (stepCount == 3)
                    {
                        shouldBreak = true;
                    }
                }
                else if (eventType == "break" && shouldBreak)
                {
                    // 触发break
                    node.SetBreak();
                }
            };

            node.OnCompleted += () => completed = true;

            // 准备输入参数
            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("start", "起始值"), new NumberValue(1) as IValue },
                { new Label("end", "结束值"), new NumberValue(10) as IValue },
                { new Label("step", "步长"), new NumberValue(1) as IValue }
            };

            // 启动循环
            await node.StartAsync(inputs);

            // 验证结果
            Assert.Equal(3, stepCount); // 应该只执行3次循环
            Assert.True(completed);
            
            var status = node.GetStatus();
            Assert.Equal(3, status.currentIndex);
            Assert.False(status.isRunning);
        }

        [Fact]
        public async Task TestForNodeWithContinue()
        {
            var node = new ForNode();
            var stepCount = 0;
            var continueCount = 0;
            var completed = false;
            var shouldContinue = false;

            // 设置事件处理器
            node.OnNodeEvent += async (eventType) =>
            {
                if (eventType == "step")
                {
                    stepCount++;
                    // 在第2次循环时设置continue标志
                    if (stepCount == 2)
                    {
                        shouldContinue = true;
                    }
                }
                else if (eventType == "continue" && shouldContinue)
                {
                    continueCount++;
                    shouldContinue = false;
                    // 这里可以添加continue逻辑
                }
            };

            node.OnCompleted += () => completed = true;

            // 准备输入参数
            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("start", "起始值"), new NumberValue(1) as IValue },
                { new Label("end", "结束值"), new NumberValue(5) as IValue },
                { new Label("step", "步长"), new NumberValue(1) as IValue }
            };

            // 启动循环
            await node.StartAsync(inputs);

            // 验证结果
            Assert.Equal(5, stepCount); // 应该执行5次循环
            Assert.Equal(1, continueCount); // 应该触发1次continue
            Assert.True(completed);
            
            var status = node.GetStatus();
            Assert.Equal(5, status.currentIndex);
            Assert.False(status.isRunning);
        }

        [Fact]
        public async Task TestForNodeInvalidParameters()
        {
            var node = new ForNode();

            // 测试起始值大于结束值
            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("start", "起始值"), new NumberValue(10) as IValue },
                { new Label("end", "结束值"), new NumberValue(5) as IValue }
            };

            await Assert.ThrowsAsync<ArgumentException>(async () => {
                await node.StartAsync(inputs);
            });
        }

        [Fact]
        public async Task TestForNodeDoubleStart()
        {
            var node = new ForNode();
            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("start", "起始值"), new NumberValue(1) as IValue },
                { new Label("end", "结束值"), new NumberValue(5) as IValue }
            };

            // 第一次启动
            var task1 = node.StartAsync(inputs);
            
            // 第二次启动应该抛出异常
            await Assert.ThrowsAsync<InvalidOperationException>(async () => {
                await node.StartAsync(inputs);
            });

            // 等待第一次完成
            await task1;
        }

        [Fact]
        public async Task TestForNodeGetCurrentOutputs()
        {
            var node = new ForNode();
            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("start", "起始值"), new NumberValue(1) as IValue },
                { new Label("end", "结束值"), new NumberValue(3) as IValue }
            };

            // 启动循环
            var task = node.StartAsync(inputs);
            
            // 等待一小段时间，检查输出
            await Task.Delay(50);
            var outputs = node.GetCurrentOutputs();
            
            // 验证输出包含预期的键
            Assert.True(outputs.ContainsKey(new Label("currentIndex", "当前索引")));
            Assert.True(outputs.ContainsKey(new Label("isFirst", "是否第一次")));
            Assert.True(outputs.ContainsKey(new Label("isLast", "是否最后一次")));
            
            // 等待完成
            await task;
        }

        [Fact]
        public async Task TestForNodeStatus()
        {
            var node = new ForNode();
            
            // 初始状态
            var status = node.GetStatus();
            Assert.Equal(0, status.currentIndex);
            Assert.False(status.isRunning);

            var inputs = new Dictionary<Label, IValue>
            {
                { new Label("start", "起始值"), new NumberValue(1) as IValue },
                { new Label("end", "结束值"), new NumberValue(3) as IValue }
            };
            
            // 启动循环
            var task = node.StartAsync(inputs);
            
            // 等待一小段时间，检查运行状态
            await Task.Delay(50);
            status = node.GetStatus();
            Assert.True(status.isRunning);
            
            // 等待完成
            await task;
            
            // 最终状态
            status = node.GetStatus();
            Assert.Equal(3, status.currentIndex);
            Assert.False(status.isRunning);
        }
    }
} 