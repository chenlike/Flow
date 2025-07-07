using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.Interface
{
    public interface IValue:IEquatable<IValue>
    {
        T GetValue<T>();

        bool SetValue<T>(T value);

        bool IsValid<T>(T value);

        string ToString();
    }
}
