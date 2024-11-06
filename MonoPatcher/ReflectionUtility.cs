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
        /// Retrieves an array of parameter types from a method.
        /// </summary>
        public static Type[] GetParameterTypes(MethodInfo method)
        {
            var prs = method.GetParameters();
            var result = new Type[prs.Length];
            for (var i = 0; i < prs.Length; i++)
                result[i] = prs[i].ParameterType;
            return result;
        }

        /// <summary>
        /// Retrieves a method from a Type, matching args.
        /// </summary>
        public static MethodInfo GetMethod(string methodName, Type type, Type[] args)
        {
            var methods = type.GetMethods(DefaultBindingFlags);
            foreach (var method in methods)
            {
                var paramTypes = GetParameterTypes(method);
                if (!Utility.ArraysMatch(paramTypes, args))
                    continue;
                if (method.Name == methodName)
                    return method;
            }
            return null;
        }

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

        /// <summary>
        /// Retrieves a property from a Type.
        /// </summary>
        public static PropertyInfo GetProperty(string propName, Type type)
        {
            var props = type.GetProperties(DefaultBindingFlags);
            foreach (var prop in props)
            {
                if (prop.Name == propName)
                    return prop;
            }
            return null;
        }

    }
}
