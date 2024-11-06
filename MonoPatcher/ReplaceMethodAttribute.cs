using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MonoPatcherLib
{
    /// <summary>
    /// Overrides a method with this.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class ReplaceMethodAttribute : PatchAttribute
    {
        public Type Type;
        public MethodInfo MethodToReplace;

        public ReplaceMethodAttribute(Type type)
        {
            Type = type;
            MethodToReplace = null;
        }

        public ReplaceMethodAttribute(Type type, string method)
        {
            Type = type;
            MethodToReplace = ReflectionUtility.GetMethod(method, type);
        }

        public ReplaceMethodAttribute(Type type, string method, Type[] argTypes)
        {
            Type = type;
            MethodToReplace = ReflectionUtility.GetMethod(method, type, argTypes);
        }

        public override void Apply(object replacement)
        {
            var methodReplacement = replacement as MethodInfo;
            if (MethodToReplace == null)
            {
                var methods = Type.GetMethods(ReflectionUtility.DefaultBindingFlags);
                MethodToReplace = Utility.FindEquivalentMethod(methodReplacement, methods);
            }
            MonoPatcher.ReplaceMethod(MethodToReplace, methodReplacement);
        }
    }
}
