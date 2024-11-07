using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPatcherLib
{
    public class Version
    {
        public enum CompareResult
        {
            Higher,
            Lower,
            Equal
        }

        public int[] Array;
        public int Major => Array[0];
        public int Minor => Array[1];
        public int Patch => Array[2];

        public Version(string version)
        {
            var split = version.Split('.');
            if (split.Length != 3)
                throw new FormatException("Invalid version format");
            Array = new int[3];
            for(var i = 0; i < 3; i++)
            {
                Array[i] = int.Parse(split[i]);
            }
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Patch}";
        }

        /// <summary>
        /// Check if this version is higher, equal or lower compared to compareVersion.
        /// </summary>
        public CompareResult CompareTo(string compareVersion)
        {
            return CompareTo(new Version(compareVersion));
        }

        /// <summary>
        /// Check if this version is higher, equal or lower compared to compareVersion.
        /// </summary>
        public CompareResult CompareTo(Version compareVersion)
        {
            for(var i = 0; i < 3; i++)
            {
                if (compareVersion.Array[i] < Array[i])
                    return CompareResult.Higher;
                else if (compareVersion.Array[i] > Array[i])
                    return CompareResult.Lower;
            }
            return CompareResult.Equal;
        }
    }
}
