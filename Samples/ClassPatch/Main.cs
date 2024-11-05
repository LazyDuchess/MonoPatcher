using MonoPatcherLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassPatch
{
    [Plugin]
    public class Main
    {
        public Main()
        {
            MonoPatcher.PatchAll();
        }
    }
}
