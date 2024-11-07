using Sims3.Gameplay.EventSystem;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace MonoPatcherLib.Internal
{
    public static class Hooking
    {
        [DllImport("Sims3Common.dll")]
        public static extern void ReplaceMethodIL(IntPtr methodPtr, IntPtr ilBegin, int ilSize);
    }
}
