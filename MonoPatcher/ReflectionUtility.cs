using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MonoPatcherLib
{
    public static class ReflectionUtility
    {
        public const BindingFlags DefaultBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

        /// <summary>
        /// Retrieves a method from a Type.
        /// </summary>
        public static MethodInfo GetMethod(string methodName, Type type)
        {
            var methods = type.GetMethods(DefaultBindingFlags);
            foreach (var method in methods)
            {
                if (method.Name == methodName)
                    return method;
            }
            return null;
        }

    }
}
