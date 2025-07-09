using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.Common
{
    public class Label
    {
        public string Code { get; }
        public string Name { get; }

        public Label(string code, string name = "")
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Name = name ?? "";
        }

        public override string ToString()
        {
            // 如果Name为空，则返回Code
            if (string.IsNullOrEmpty(Name))
            {
                return Code;
            }
            return Name;
        }
    }
}
