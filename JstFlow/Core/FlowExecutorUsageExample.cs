using System;
using System.Collections.Generic;
using JstFlow.Core;
using JstFlow.Common;

namespace JstFlow.Core
{
    /// <summary>
    /// FlowExecutor使用示例
    /// </summary>
    public class FlowExecutorUsageExample
    {
        public static void DemonstrateManualControl()
        {
            // 假设我们已经有了一个FlowGraph
            // var flowGraph = CreateSampleFlowGraph();
            
            // 创建执行器
            // var executorRes = FlowExecutor.Create(flowGraph);
            // if (!executorRes.IsSuccess)
            // {
            //     Console.WriteLine($"创建执行器失败: {executorRes.Message}");
            //     return;
            // }
            
            // var executor = executorRes.Data;
            
            // 示例：手动控制执行流程
            Console.WriteLine("=== FlowExecutor 手动控制示例 ===");
            
            // 1. 开始执行（只执行开始节点，不自动继续）
            // executor.Start();
            // Console.WriteLine($"开始执行后，当前节点: {executor.CurrentNodeId}");
            // Console.WriteLine($"待执行任务数: {executor.GetPendingTaskCount()}");
            
            // 2. 手动执行下一个节点
            // while (executor.CanContinue())
            // {
            //     Console.WriteLine($"\n准备执行下一个节点...");
            //     Console.WriteLine($"当前节点: {executor.GetCurrentNode()?.Label?.DisplayName ?? "未知"}");
            //     Console.WriteLine($"待执行任务数: {executor.GetPendingTaskCount()}");
            //     
            //     bool executed = executor.StepNext();
            //     if (!executed)
            //     {
            //         Console.WriteLine("无法继续执行");
            //         break;
            //     }
            //     
            //     Console.WriteLine($"执行完成，当前节点: {executor.CurrentNodeId}");
            //     
            //     // 可以在这里添加用户交互
            //     Console.WriteLine("按任意键继续执行下一个节点...");
            //     Console.ReadKey();
            // }
            
            // 3. 或者一次性执行所有剩余节点
            // executor.ExecuteAll();
            
            // 4. 获取执行统计
            // var stats = executor.GetExecutionStatistics();
            // Console.WriteLine($"\n执行完成！");
            // Console.WriteLine($"总执行节点数: {stats.TotalNodesExecuted}");
            // Console.WriteLine($"是否完成: {stats.IsCompleted}");
            // Console.WriteLine($"执行历史: {string.Join(" -> ", stats.ExecutionHistory)}");
        }
        
        public static void DemonstratePauseAndResume()
        {
            Console.WriteLine("\n=== 暂停和恢复示例 ===");
            
            // 示例：暂停和恢复功能
            // var executor = CreateExecutor();
            // executor.Start();
            
            // 执行几个节点后暂停
            // for (int i = 0; i < 3; i++)
            // {
            //     executor.StepNext();
            // }
            
            // executor.Pause();
            // Console.WriteLine("执行已暂停");
            
            // 保存当前状态
            // var snapshot = executor.GetSnapshot();
            
            // 恢复执行
            // executor.Resume();
            // Console.WriteLine("执行已恢复");
            
            // 继续执行剩余节点
            // executor.ExecuteAll();
        }
        
        public static void DemonstrateSnapshot()
        {
            Console.WriteLine("\n=== 快照功能示例 ===");
            
            // 示例：使用快照保存和恢复执行状态
            // var executor = CreateExecutor();
            // executor.Start();
            
            // 执行几个节点
            // for (int i = 0; i < 2; i++)
            // {
            //     executor.StepNext();
            // }
            
            // 保存快照
            // var snapshot = executor.GetSnapshot();
            // Console.WriteLine("已保存执行快照");
            
            // 停止执行器
            // executor.Stop();
            // Console.WriteLine("执行器已停止");
            
            // 从快照恢复
            // executor.RestoreFromSnapshot(snapshot);
            // Console.WriteLine("已从快照恢复执行状态");
            
            // 继续执行
            // executor.ExecuteAll();
        }
    }
} 