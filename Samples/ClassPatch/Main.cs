using MonoPatcherLib;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassPatch
{
    // Classes with the [Plugin] attribute will be automatically created as soon as possible by MonoPatcher. You don't need a tuning XML to instantiate.
    [Plugin]
    public class Main
    {
        public Main()
        {
            // Applies all patches found in your mod's DLL.
            MonoPatcher.PatchAll();
        }
    }
}
