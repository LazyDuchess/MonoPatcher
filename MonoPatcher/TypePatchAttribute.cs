using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
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
        }

        private MethodInfo FindEquivalentMethod(MethodInfo method, MethodInfo[] inMethods)
        {
            foreach(var methodB in inMethods)
            {
                if (methodB.Name == method.Name)
                    return methodB;
            }
            return null;
        }
    }
}
