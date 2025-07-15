using IdGen;
using System;
using System.Collections.Generic;
using System.Text;

namespace JstFlow.Common
{
    public static class Utils
    {
        static IdGenerator generator = new IdGenerator(0);
        public static long GenId()
        {
            return generator.CreateId();
        }
    }
}
