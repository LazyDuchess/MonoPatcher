using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MonoPatcher
{
    public static class ReflectionUtility
    {
        /// <summary>
        /// Retrieves a method from a Type.
        /// </summary>
        public static MethodInfo GetMethod(string methodName, Type type)
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            foreach (var method in methods)
            {
                if (method.Name == methodName)
                    return method;
            }
            return null;
        }

    }
}
