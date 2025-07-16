using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.Core.Interfaces
{
    public interface IFlowSnapshot
    {
        string CreateSnapshot();

        void RestoreFromSnapshot(string snapshot);

    }
}
