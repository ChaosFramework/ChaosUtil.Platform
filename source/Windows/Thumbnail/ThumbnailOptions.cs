/* This file is based on Microsoft's PowerToys at GitHub
 * source commit: e10733efe0728ef7829b69821f0040205546717c
 * source file:   src/modules/launcher/Wox.Infrastructure/Image/WindowsThumbnailProvider.cs
 * source lines:  17-26
 *
 * For license information check the 'Licenses' directory of this repository.
 */

using System;

namespace ChaosUtil.Platform.Windows.Thumbnail
{
    /// <summary> Control whether or how a loaded thumbnail is scaled and whether it may be loaded from memory, cache or disk. </summary>
    [Flags]
    public enum ThumbnailOptions
    {
        /// <summary> Scale down to fit the desired size without changing aspect ratio. </summary>
        RESIZETOFIT = 0x00,

        /// <summary> Specifies that the resulting thumbnail may be larger than the desired size. </summary>
        BiggerSizeOk = 0x01,

        /// <summary> Only return a thumbnail if it's already loaded in memory. </summary>
        InMemoryOnly = 0x02,

        /// <summary> Only return icons. </summary>
        IconOnly = 0x04,

        /// <summary> Only return thumbnails. </summary>
        ThumbnailOnly = 0x08,

        /// <summary> Only fetch from disk if the icon is already cached. </summary>
        InCacheOnly = 0x10,
    }
}
