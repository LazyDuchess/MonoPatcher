using System;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;

namespace MonoPatcherLib
{
	/// <summary>
	/// Entry Point called from GameplayInitializers.XML in case of no DLL Hook.
	/// </summary>
	internal class XMLEntryPoint
	{
		public static void Init()
		{
            if (MonoPatcher.InitializationType != MonoPatcher.InitializationTypes.None) return;
            MonoPatcher.Initialize(MonoPatcher.InitializationTypes.XML);
        }
	}
}