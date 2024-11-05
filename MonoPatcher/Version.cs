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
        /// Check if the version in compareVersion is higher, equal or lower than this.
        /// </summary>
        public CompareResult Compare(string compareVersion)
        {
            return Compare(new Version(compareVersion));
        }

        /// <summary>
        /// Check if the version in compareVersion is higher, equal or lower than this.
        /// </summary>
        public CompareResult Compare(Version compareVersion)
        {
            for(var i = 0; i < 3; i++)
            {
                if (compareVersion.Array[i] > Array[i])
                    return CompareResult.Higher;
                else if (compareVersion.Array[i] < Array[i])
                    return CompareResult.Lower;
            }
            return CompareResult.Equal;
        }
    }
}
