/* This file is based on Microsoft's PowerToys at GitHub
 * source commit: e10733efe0728ef7829b69821f0040205546717c
 * source file:   src/modules/launcher/Wox.Infrastructure/Image/WindowsThumbnailProvider.cs
 * source lines:  83-169
 *
 * For license information check the 'Licenses' directory of this repository.
 */

using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace ChaosUtil.Platform.Windows.Thumbnail
{
    public static class WindowsThumbnailProvider
    {
        public static Bitmap GetThumbnail(string fileName, int width, int height, ThumbnailOptions options)
        {
            IntPtr hBitmap = IntPtr.Zero;

            if (Path.GetExtension(fileName).Equals(".lnk", StringComparison.OrdinalIgnoreCase))
            {
                hBitmap = ExtractIconToHBitmap(fileName);
            }
            else
            {
                hBitmap = GetHBitmap(Path.GetFullPath(fileName), width, height, options);
            }

            try
            {
                return Image.FromHbitmap(hBitmap);
            }
            finally
            {
                // delete HBitmap to avoid memory leaks
                NativeMethods.DeleteObject(hBitmap);
            }
        }

        static IntPtr GetHBitmap(string fileName, int width, int height, ThumbnailOptions options)
        {
            IntPtr hBitmap = IntPtr.Zero;
            IShellItem nativeShellItem = null;

            try
            {
                Guid shellItem2Guid = new Guid("7E9FB0D3-919F-4307-AB2E-9B1860310C93");
                int retCode = NativeMethods.SHCreateItemFromParsingName(fileName, IntPtr.Zero, ref shellItem2Guid, out nativeShellItem);

                if (retCode != 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Error while creating item. retCode:{retCode} ");
                    throw Marshal.GetExceptionForHR(retCode);
                }

                NativeSize nativeSize = new NativeSize
                {
                    Width = width,
                    Height = height,
                };

                HResult hr = ((IShellItemImageFactory)nativeShellItem).GetImage(nativeSize, options, out hBitmap);

                // if extracting image thumbnail and failed, extract shell icon
                if (options == ThumbnailOptions.ThumbnailOnly && hr == HResult.ExtractionFailed)
                {
                    hr = ((IShellItemImageFactory)nativeShellItem).GetImage(nativeSize, ThumbnailOptions.IconOnly, out hBitmap);
                }

                if (hr != HResult.Ok)
                {
                    throw Marshal.GetExceptionForHR((int)hr);
                }

                return hBitmap;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error while extracting thumbnail for {fileName}");
                throw;
            }
            finally
            {
                if (nativeShellItem != null)
                {
                    Marshal.ReleaseComObject(nativeShellItem);
                }
            }
        }

        static IntPtr ExtractIconToHBitmap(string fileName)
        {
            // Extracts the icon associated with the file
            using (System.Drawing.Icon thumbnailIcon = System.Drawing.Icon.ExtractAssociatedIcon(fileName))
            {
                // Convert to Bitmap
                using (System.Drawing.Bitmap bitmap = thumbnailIcon.ToBitmap())
                {
                    return bitmap.GetHbitmap();
                }
            }
        }
    }
}
