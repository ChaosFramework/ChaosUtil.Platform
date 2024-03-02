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
    [Flags]
    public enum ThumbnailOptions
    {
        RESIZETOFIT = 0x00,
        BiggerSizeOk = 0x01,
        InMemoryOnly = 0x02,
        IconOnly = 0x04,
        ThumbnailOnly = 0x08,
        InCacheOnly = 0x10,
    }
}
