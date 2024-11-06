using System;
using System.Collections.Generic;
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
    }
}
