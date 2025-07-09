using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.Common
{
    public class Label
    {
        public string Code { get; }
        public string Display { get; }

        public Label(string code, string name = "")
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Display = name ?? code;
        }

        public override string ToString()
        {
            return Display;
        }
    }
}
