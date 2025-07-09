using JstFlow.Common;
using System;
using System.Collections.Generic;
using System.Text;
using JstFlow.Internal.Metas;

namespace JstFlow.Internal.NodeMeta
{
    public class FlowNodeMeta
    {

        public Label Name { get; set; }

        public Type NodeImplType { get; set; }

        /// <summary>
        /// 节点类型
        /// </summary>
        public NodeKind Kind { get; set; }

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
