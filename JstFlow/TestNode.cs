using JstFlow.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JstFlow
{
    [FlowNode("测试节点")]
    public class TestNode
    {

        [Input("名称",Required = true)]
        public string Name { get; set; }

        [Input("年龄")]
        public decimal Age { get; set; }


        [Output("联合信息")]
        public string CombineInfo { get; set; }

        [Emit("输出")]
        public event Action TTT;


        [Signal("输入")]
        public void TestA()
        {

        }



    }
}
