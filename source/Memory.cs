using System;
using System.Runtime.InteropServices;

namespace ChaosUtil.Platform
{
    /// <summary> Provides utility functions for buffer manipulation. </summary>
    public static class Memory
    {
        /// <summary> Copies bytes from one buffer to another. </summary>
        /// <param name="dest"> The destination buffer. </param>
        /// <param name="source"> The source buffer. </param>
        /// <param name="count"> The number of bytes to be copied. </param>
        /// <returns> The destination buffer (<paramref name="dest"/>). </returns>
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr Copy(IntPtr dest, IntPtr source, UIntPtr count);
    }
}
