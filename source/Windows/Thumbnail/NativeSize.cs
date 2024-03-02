/* This file is based on Microsoft's PowerToys at GitHub
 * source commit: e10733efe0728ef7829b69821f0040205546717c
 * source file:   src/modules/launcher/Wox.Infrastructure/Image/WindowsThumbnailProvider.cs
 * source line:   66-81
 *
 * For license information check the 'Licenses' directory of this repository.
 */

using System.Runtime.InteropServices;

namespace ChaosUtil.Platform.Windows.Thumbnail
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeSize
    {
        private int width;
        private int height;

        public int Width
        {
            set { width = value; }
        }

        public int Height
        {
            set { height = value; }
        }
    }
}
