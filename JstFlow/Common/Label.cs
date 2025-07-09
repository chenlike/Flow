using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.Common
{
    public class Label
    {
        public string Code { get; set; }
        public string DisplayName { get; set; }

        public Label(string code, string name = "")
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            DisplayName = name ?? code;
        }

        // 默认构造函数，用于 JSON 反序列化
        public Label()
        {
            Code = string.Empty;
            DisplayName = string.Empty;
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
