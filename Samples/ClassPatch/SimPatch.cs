using System;
using System.Collections.Generic;
using System.Text;
using MonoPatcherLib;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Controllers;

namespace ClassPatch
{
    // TypePatch means all properties and methods that match those that can be found in the original class will be overriden.
    [TypePatch(typeof(Sim))]
    public class SimPatch
    {
        // Display first name only as tooltip.
        public string ToTooltipString()
        {
            var sim = (Sim)(this as object);
            if (sim == null || sim.SimDescription == null)
                return string.Empty;
            return sim.SimDescription.FirstName;
        }

        // Don't spawn clothing piles.
        public void SpawnClothingPileOneShot()
        {

        }
    }
}
