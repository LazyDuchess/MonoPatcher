using MonoPatcherLib.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPatcherLib
{
    /// <summary>
    /// Forces JIT recompilation of any methods called inside an using block. Needs ASI to work - InitializationType must be CPP.
    /// </summary>
    public class ForceRecompilation : IDisposable
    {
        public static bool Active { get; private set; }

        public ForceRecompilation()
        {
            Active = true;
            if (MonoPatcher.InitializationType == MonoPatcher.InitializationTypes.CPP)
                Hooking.ForceJIT(true);
        }

        public void Dispose()
        {
            Active = false;
            if (MonoPatcher.InitializationType == MonoPatcher.InitializationTypes.CPP)
                Hooking.ForceJIT(false);
        }
    }
}
