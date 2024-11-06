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
    public class TypePatchAttribute : Attribute
    {
        public Type Type;

        public TypePatchAttribute(Type typeToPatch)
        {
            Type = typeToPatch;
        }

        public void Apply(Type replacementType)
        {
            var replacementMethods = replacementType.GetMethods(ReflectionUtility.DefaultBindingFlags);
            var originalMethods = Type.GetMethods(ReflectionUtility.DefaultBindingFlags);

            foreach(var method in replacementMethods)
            {
                var equivalent = FindEquivalentMethod(method, originalMethods);
                if (equivalent != null)
                    MonoPatcher.ReplaceMethod(equivalent, method);
            }

            var replacementProps = replacementType.GetProperties(ReflectionUtility.DefaultBindingFlags);
            var originalProps = Type.GetProperties(ReflectionUtility.DefaultBindingFlags);

            foreach(var prop in replacementProps)
            {
                var equivalent = FindEquivalentProperty(prop, originalProps);
                if (equivalent != null)
                    MonoPatcher.ReplaceProperty(equivalent, prop);
            }
        }

        private PropertyInfo FindEquivalentProperty(PropertyInfo prop, PropertyInfo[] inProps)
        {
            foreach (var propB in inProps)
            {
                if (propB.Name == prop.Name)
                    return prop;
            }
            return null;
        }

        private MethodInfo FindEquivalentMethod(MethodInfo method, MethodInfo[] inMethods)
        {
            foreach(var methodB in inMethods)
            {
                if (methodB.Name == method.Name && Utility.ArraysMatch(ReflectionUtility.GetParameterTypes(method), ReflectionUtility.GetParameterTypes(methodB)))
                    return methodB;
            }
            return null;
        }
    }
}
