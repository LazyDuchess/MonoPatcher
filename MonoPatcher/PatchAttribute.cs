using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPatcherLib
{
    public abstract class PatchAttribute : Attribute
    {
        public abstract void Apply(object replacement);
    }
}
