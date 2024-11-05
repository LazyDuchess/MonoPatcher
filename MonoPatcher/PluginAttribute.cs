using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPatcher
{
    /// <summary>
    /// Automatically constructs this class on MonoPatcher initialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PluginAttribute : Attribute
    {
    }
}
