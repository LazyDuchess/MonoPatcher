using MonoPatcherLib.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPatcherLib
{
    public class ForceRecompilation : IDisposable
    {
        public static bool Active { get; private set; }

        public ForceRecompilation()
        {
            Active = true;
            Hooking.ForceJIT(true);
        }

        public void Dispose()
        {
            Active = false;
            Hooking.ForceJIT(false);
        }
    }
}
