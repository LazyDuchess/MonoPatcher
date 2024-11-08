using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPatcherLib
{
    public class LoadedPlugin
    {
        public Type Type;
        public object Instance;
        public bool PatchOnInitialize = true;

        internal LoadedPlugin(Type type, bool patchOnInitialize)
        {
            Type = type;
            PatchOnInitialize = patchOnInitialize;
        }

        internal void Initialize()
        {
            Instance = Activator.CreateInstance(Type);
            if (PatchOnInitialize)
                MonoPatcher.PatchAll(Type.Assembly);
        }
    }
}
