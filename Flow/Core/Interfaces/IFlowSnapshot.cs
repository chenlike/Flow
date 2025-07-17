using System;
using System.Collections.Generic;
using System.Text;

namespace Flow.Core.Interfaces
{
    public interface IFlowSnapshot
    {
        string CreateSnapshot();

        void RestoreFromSnapshot(string snapshot);

    }
}
