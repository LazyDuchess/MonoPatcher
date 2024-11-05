using System;
using System.Collections.Generic;
using System.Text;
using MonoPatcherLib;
using Sims3.Gameplay.Actors;

namespace ClassPatch
{
    [TypePatch(typeof(Sim))]
    public class SimPatch
    {
        public string ToTooltipString()
        {
            return "MonoPatched Sim";
        }
    }
}
