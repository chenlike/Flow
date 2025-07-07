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
    }
}
