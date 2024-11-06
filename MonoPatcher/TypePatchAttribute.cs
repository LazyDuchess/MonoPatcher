using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MonoPatcherLib
{
    /// <summary>
    /// Overrides matching methods in the target Type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
    public class TypePatchAttribute : PatchAttribute
    {
        public Type Type;

        public TypePatchAttribute(Type typeToPatch)
        {
            Type = typeToPatch;
        }

        public override void Apply(object replacement)
        {
            Type replacementType = replacement as Type;
            var replacementMethods = replacementType.GetMethods(ReflectionUtility.DefaultBindingFlags);
            var originalMethods = Type.GetMethods(ReflectionUtility.DefaultBindingFlags);

            foreach(var method in replacementMethods)
            {
                // Don't duplicate patches.
                if (method.GetCustomAttributes(typeof(PatchAttribute), false).Length > 0) continue;
                if (method.DeclaringType != replacementType) continue;
                var equivalent = Utility.FindEquivalentMethod(method, originalMethods);
                if (equivalent != null)
                    MonoPatcher.ReplaceMethod(equivalent, method);
            }

            var replacementProps = replacementType.GetProperties(ReflectionUtility.DefaultBindingFlags);
            var originalProps = Type.GetProperties(ReflectionUtility.DefaultBindingFlags);

            foreach(var prop in replacementProps)
            {
                // Don't duplicate patches.
                if (prop.GetCustomAttributes(typeof(PatchAttribute), false).Length > 0) continue;
                if (prop.DeclaringType != replacementType) continue;
                var equivalent = Utility.FindEquivalentProperty(prop, originalProps);
                if (equivalent != null)
                    MonoPatcher.ReplaceProperty(equivalent, prop);
            }
        }
    }
}
