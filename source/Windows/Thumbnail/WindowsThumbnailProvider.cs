/* This file is based on Microsoft's PowerToys at GitHub
 * source commit: e10733efe0728ef7829b69821f0040205546717c
 * source file:   src/modules/launcher/Wox.Infrastructure/Image/WindowsThumbnailProvider.cs
 * source lines:  83-169
 *
 * For license information check the 'Licenses' directory of this repository.
 */

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace ChaosUtil.Platform.Windows.Thumbnail
{
    /// <summary> Provides functionality to load a thumbnail for a file. </summary>
    public static class WindowsThumbnailProvider
    {
        /// <summary> Returns a <see cref="Bitmap"/> containing the thumbnail for the given <paramref name="fileName"/>. </summary>
        /// <param name="fileName"> The path to the file to retrieve the thumbnail for. </param>
        /// <param name="width">
        ///     The desired width of the thumbnail.
        ///     Depending on <paramref name="options"/> the actual width may differ.
        /// </param>
        /// <param name="height">
        ///     The desired height of the thumbnail.
        ///     Depending on <paramref name="options"/> the actual height may differ.
        /// </param>
        /// <param name="options">
        ///     Control whether or how a loaded thumbnail is scaled
        ///     and whether it may be loaded from memory, cache or disk.
        /// </param>
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
                Bitmap bm = Image.FromHbitmap(hBitmap);
                switch (bm.PixelFormat)
                {
                    case PixelFormat.Format32bppRgb:
                        {
                            /* From https://learn.microsoft.com/en-us/dotnet/api/system.drawing.imaging.pixelformat
                             *     Specifies that the format is 32 bits per pixel;
                             *     8 bits each are used for the red, green, and blue components.
                             *     The remaining 8 bits are not used.
                             *
                             * Apparently in this case the unused 8 bits actually hold a valid alpha channel...
                             */

                            Bitmap argb = ConvertRGB32toARGB32(bm);
                            bm.Dispose();
                            return argb;
                        }

                    default:
                        // TODO: Figure out whether there are more formats that actually contain alpha information.
                        return bm;
                }
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
                System.Diagnostics.Debug.WriteLine($"Error while extracting thumbnail for {fileName}: {ex.Message}");
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

        static Bitmap ConvertRGB32toARGB32(Bitmap rgb32)
        {
            const PixelFormat RGB32 = PixelFormat.Format32bppRgb;
            const PixelFormat ARGB32 = PixelFormat.Format32bppArgb;

            if (rgb32.PixelFormat != RGB32)
                throw new ArgumentException($"The provided bitmap must have the {RGB32} format.", nameof(rgb32));

            Bitmap argb32 = new Bitmap(rgb32.Width, rgb32.Height, ARGB32);

            Rectangle bounds = new Rectangle(0, 0, rgb32.Width, rgb32.Height);
            BitmapData scanRgb = rgb32.LockBits(bounds, ImageLockMode.ReadOnly, RGB32);
            BitmapData scanArgb = argb32.LockBits(bounds, ImageLockMode.WriteOnly, ARGB32);

            UIntPtr bytesPerRow = new UIntPtr((uint)rgb32.Width * 4u);
            for (int row = 0; row < scanRgb.Height; row++)
            {
                IntPtr rgbPos = IntPtr.Add(scanRgb.Scan0, row * scanRgb.Stride);
                IntPtr argbPos = IntPtr.Add(scanArgb.Scan0, row * scanArgb.Stride);
                Memory.Copy(argbPos, rgbPos, bytesPerRow);
            }

            rgb32.UnlockBits(scanRgb);
            argb32.UnlockBits(scanArgb);

            return argb32;
        }
    }
}
