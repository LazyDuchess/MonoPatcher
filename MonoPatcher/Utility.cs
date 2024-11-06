﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MonoPatcherLib
{
    public static class Utility
    {
        public static bool ArraysMatch(Array array1, Array array2)
        {
            if (array1.Length != array2.Length) return false;
            for(var i=0;i<array1.Length; i++)
            {
                if (array1.GetValue(i) != array2.GetValue(i)) return false;
            }
            return true;
        }

        public static PropertyInfo FindEquivalentProperty(PropertyInfo prop, PropertyInfo[] inProps)
        {
            foreach (var propB in inProps)
            {
                if (propB.Name == prop.Name)
                    return prop;
            }
            return null;
        }

        public static MethodInfo FindEquivalentMethod(MethodInfo method, MethodInfo[] inMethods)
        {
            foreach (var methodB in inMethods)
            {
                if (methodB.Name == method.Name && Utility.ArraysMatch(ReflectionUtility.GetParameterTypes(method), ReflectionUtility.GetParameterTypes(methodB)))
                    return methodB;
            }
            return null;
        }
    }
}