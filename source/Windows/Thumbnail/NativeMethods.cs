/* This file is based on Microsoft's PowerToys at GitHub
 * source commit: ec8ead818303384c1d98c17342ebda84c5e75187
 * source file:   src/modules/launcher/Wox.Infrastructure/Image/NativeMethods.cs
 *
 * For license information check the 'Licenses' directory of this repository.
 */

using System;
using System.Runtime.InteropServices;

namespace ChaosUtil.Platform.Windows.Thumbnail
{
    static class NativeMethods
    {
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int SHCreateItemFromParsingName(
            [MarshalAs(UnmanagedType.LPWStr)] string path,
            IntPtr pbc,
            ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out IShellItem shellItem);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr hObject);
    }
}
