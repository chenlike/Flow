using JstFlow.Common;
using System;
using System.Collections.Generic;
using System.Text;
using JstFlow.Core.Metas;

namespace JstFlow.Core.NodeMeta
{
    public class FlowNodeInfo
    {

        /// <summary>
        /// 节点的唯一标识
        /// </summary>
        public long Id { get; set; } = Utils.GenId();

        /// <summary>
        /// 节点名称
        /// </summary>
        public Label Label { get; set; }

        /// <summary>
        /// 节点实现类型
        /// </summary>
        public Type NodeImplType { get; set; }

        /// <summary>
        /// 节点实现类型全名
        /// </summary>
        public string NodeImplTypeFullName { get; set; }

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
