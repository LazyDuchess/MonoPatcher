using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPatcherLib
{
    /// <summary>
	/// Entry Point called from a DLL Hook, as early as possible so that more methods can be patched.
	/// </summary>
    internal class DLLEntryPoint
    {
        public DLLEntryPoint()
        {
            AppDomain.CurrentDomain.SetData("DLLEntryPoint", this);
            if (MonoPatcher.InitializationType != MonoPatcher.InitializationTypes.None) return;
            MonoPatcher.Initialize(MonoPatcher.InitializationTypes.CPP);
        }
    }
}
