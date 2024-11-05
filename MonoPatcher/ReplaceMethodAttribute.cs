using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MonoPatcher
{
    /// <summary>
    /// Overrides a method with this.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class ReplaceMethodAttribute : Attribute
    {
        public Type Type;
        public MethodInfo MethodToReplace;
        public ReplaceMethodAttribute(Type type, string method)
        {
            Type = type;
            MethodToReplace = ReflectionUtility.GetMethod(method, type);
        }

        public ReplaceMethodAttribute(Type type, MethodInfo method)
        {
            Type = type;
            MethodToReplace = method;
        }

        public void Apply(MethodInfo replacement)
        {
            MonoPatcher.ReplaceMethod(MethodToReplace, replacement);
        }
    }
}
