using Sims3.Gameplay.EventSystem;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace MonoPatcherLib.Internal
{
    internal static class Hooking
    {
        public static Dictionary<IntPtr, WeavedMethod> WeavedMethods = new Dictionary<IntPtr, WeavedMethod>();
        [DllImport("MonoPatcher.asi")]
        public static extern void ReplaceMethodIL(IntPtr methodPtr, IntPtr ilBegin, int ilSize);
        [DllImport("MonoPatcher.asi")]
        public static extern void ForceJIT(bool force);
        public class WeavedMethod : IDisposable
        {
            public IntPtr Allocation;
            public int Size;

            public WeavedMethod(IntPtr alloc, int size)
            {
                Allocation = alloc;
                Size = size;
            }

            public byte[] GetBytes()
            {
                var bytes = new byte[Size];
                Marshal.Copy(Allocation, bytes, 0, Size);
                return bytes;
            }

            public void Dispose()
            {
                Marshal.FreeHGlobal(Allocation);
            }
        }
    }
}
