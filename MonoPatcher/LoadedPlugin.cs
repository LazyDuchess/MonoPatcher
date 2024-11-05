using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPatcher
{
    public class LoadedPlugin
    {
        Type Type;
        object Instance;

        internal LoadedPlugin(Type type)
        {
            Type = type;
        }

        internal void Initialize()
        {
            Instance = Activator.CreateInstance(Type);
        }
    }
}
