using MonoPatcherLib;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.OnlineDating;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SamplePatches
{
    // Classes with the [Plugin] attribute will be automatically created as soon as possible by MonoPatcher. You don't need a tuning XML to instantiate.
    [Plugin]
    public class Main
    {
        public Main()
        {
            // Applies all patch attributes found in your mod's DLL. This should always be done in your plugin constructor, before most code is compiled.
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

        // Replaces the OnlineDatingProfile body type method to make the weight threshold higher.
        [ReplaceMethod(typeof(OnlineDatingProfile), nameof(OnlineDatingProfile.GetDefaultBodyType))]
        private DatingBodyTypes GetDefaultBodyTypePatch()
        {
            var profile = (OnlineDatingProfile)(this as object);
            // From > 0.0f to 0.5f
            if (profile.mSimDescription.Weight > 0.5f)
            {
                return DatingBodyTypes.MoreToLove;
            }
            if (profile.mSimDescription.Fitness > 0.5f)
            {
                return DatingBodyTypes.Athletic;
            }
            return DatingBodyTypes.Slim;
        }

        // Same as the above patch, but using IL weaving. For more advanced users - harder and limited, but more compatible as multiple IL patches can be stacked together.
        private void ILPatch_GetDefaultBodyType()
        {
            // Can only do IL weaving if the ASI is loaded.
            if (MonoPatcher.InitializationType != MonoPatcher.InitializationTypes.CPP) return;

            var GetDefaultBodyTypeMethod = typeof(OnlineDatingProfile).GetMethod(nameof(OnlineDatingProfile.GetDefaultBodyType), BindingFlags.NonPublic | BindingFlags.Instance);

            // Get the bytecode from the method
            var il = MonoPatcher.GetIL(GetDefaultBodyTypeMethod);

            // Look for the instruction that loads a 0.0 float

            var search = new byte[5];
            // ldc.r4
            search[0] = 0x22;
            // float 0
            search[1] = 0x0;
            search[1] = 0x0;
            search[1] = 0x0;
            search[1] = 0x0;

            var weightLocation = Utility.FindInByteArray(il, search);

            if (weightLocation == -1) return;

            // Replace the 0.0 float with 0.5
            Array.Copy(BitConverter.GetBytes(0.5f), 0, il, weightLocation + 1, 4);

            // Patch the method
            MonoPatcher.ReplaceIL(GetDefaultBodyTypeMethod, il);
        }
    }
}
