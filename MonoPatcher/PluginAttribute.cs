using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPatcherLib
{
    /// <summary>
    /// Automatically constructs this class on MonoPatcher initialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PluginAttribute : Attribute
    {
        public bool ApplyPatchesAutomatically = true;
        public PluginAttribute()
        {

        }

        public PluginAttribute(bool applyPatchesAutomatically)
        {
            ApplyPatchesAutomatically = applyPatchesAutomatically;
        }
    }
}
