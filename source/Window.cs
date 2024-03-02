using System;
using System.Runtime.InteropServices;

namespace ChaosUtil.Platform
{
    /// <summary> Provides utility methods for working with windows. </summary>
    public static class Window
    {
        /// <summary> Retrieves the window handle to the active window attached to the calling thread's message queue. </summary>
        /// <returns> The handle to the active window attached to the calling thread's message queue. </returns>
        /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getactivewindow"/>
        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();
    }
}
