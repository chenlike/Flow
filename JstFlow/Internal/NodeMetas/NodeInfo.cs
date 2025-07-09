using JstFlow.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.Internal.NodeMeta
{
    public class NodeInfo
    {

        public Label Name { get; set; }

        public Type NodeType { get; set; }

        /// <summary>
        /// 输入字段
        /// </summary>
        public List<InputField> InputFields { get; set; }

        /// <summary>
        /// 输出字段
        /// </summary>
        public List<OutputField> OutputFields { get; set; }


        /// <summary>
        /// 输入信号
        /// </summary>
        public List<SignalInfo> Signals { get; set; }

        /// <summary>
        /// 输出事件
        /// </summary>
        public List<EmitInfo> Emits { get; set; }






    }
}
