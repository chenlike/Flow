using JstFlow.Common;
using JstFlow.Internal.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.Interface.Models
{
    public class NodeExecuteResult
    {
        public IDictionary<Label, IValue> Outputs { get; set; }
        public Label ActionToExecute { get; set; }
        
        /// <summary>
        /// 执行状态
        /// </summary>
        public string Status { get; set; } = "normal";
        
        /// <summary>
        /// 信号类型（break、continue、normal 等）
        /// </summary>
        public string Signal { get; set; }
        
        /// <summary>
        /// 是否继续执行
        /// </summary>
        public bool ShouldContinue { get; set; } = true;
        
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
    }
}
