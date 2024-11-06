using MonoPatcherLib;
using Sims3.Gameplay.CAS;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamplePatches
{
    // Classes with the [Plugin] attribute will be automatically created as soon as possible by MonoPatcher. You don't need a tuning XML to instantiate.
    [Plugin]
    public class Main
    {
        public Main()
        {
            // Applies all patches found in your mod's DLL. This should always be done in your plugin constructor, before most code is compiled.
            MonoPatcher.PatchAll();
        }

        // Replaces the getter for Sim full names.
        [ReplaceProperty(typeof(SimDescriptionCore), nameof(SimDescriptionCore.FullName))]
        private string SimFullNamePatch
        {
            get
            {
                return "Mono Patcher";
            }
        }
    }
}
