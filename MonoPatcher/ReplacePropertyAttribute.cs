using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MonoPatcherLib
{
    /// <summary>
    /// Overrides a property with this.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class ReplacePropertyAttribute : Attribute
    {
        public Type Type;
        public PropertyInfo PropertyToReplace;
        public ReplacePropertyAttribute(Type type, string property)
        {
            Type = type;
            PropertyToReplace = ReflectionUtility.GetProperty(property, type);
        }

        public ReplacePropertyAttribute(Type type, PropertyInfo property)
        {
            Type = type;
            PropertyToReplace = property;
        }

        public void Apply(PropertyInfo replacement)
        {
            MonoPatcher.ReplaceProperty(PropertyToReplace, replacement);
        }
    }
}
