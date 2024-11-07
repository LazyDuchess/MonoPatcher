using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MonoPatcherLib.Internal
{
    public sealed class ILGeneration
    {
        [DllImport("Sims3Common.dll")]
        public static extern void Test();
    }
}
