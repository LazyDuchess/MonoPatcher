using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MonoPatcherLib.Internal
{
    public sealed class Hooking
    {
        [DllImport("Sims3Common.dll")]
        public static extern void ReplaceMethodIL(IntPtr methodPtr, IntPtr ilBegin, int ilSize);
    }
}
