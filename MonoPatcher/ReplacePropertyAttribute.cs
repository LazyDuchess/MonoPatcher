using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MonoPatcherLib
{
    /// <summary>
    /// Overrides a property with this one.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class ReplacePropertyAttribute : PatchAttribute
    {
        public Type Type;
        public PropertyInfo PropertyToReplace;

        public ReplacePropertyAttribute(Type type)
        {
            Type = type;
            PropertyToReplace = null;
        }

        public ReplacePropertyAttribute(Type type, string property)
        {
            Type = type;
            PropertyToReplace = ReflectionUtility.GetProperty(property, type);
        }

        public override void Apply(object replacement)
        {
            var propertyReplacement = replacement as PropertyInfo;
            if (PropertyToReplace == null)
            {
                var props = Type.GetProperties(ReflectionUtility.DefaultBindingFlags);
                PropertyToReplace = Utility.FindEquivalentProperty(propertyReplacement, props);
            }
            MonoPatcher.ReplaceProperty(PropertyToReplace, propertyReplacement);
        }
    }
}
