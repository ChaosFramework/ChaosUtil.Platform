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
    /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/shobjidl_core/nf-shobjidl_core-ishellitemimagefactory-getimage#parameters"/>
    [Flags]
    public enum ThumbnailOptions
    {
        /// <summary> Scale down to fit the desired size without changing aspect ratio. </summary>
        /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/shobjidl_core/nf-shobjidl_core-ishellitemimagefactory-getimage#siigbf_resizetofit-0x00000000"/>
        ResizeToFit = 0x00,

        /// <summary> Specifies that the resulting thumbnail may be larger than the desired size. </summary>
        /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/shobjidl_core/nf-shobjidl_core-ishellitemimagefactory-getimage#siigbf_biggersizeok-0x00000001"/>
        BiggerSizeOk = 0x01,

        /// <summary> Fetch the thumbnail from memory if available or fallback to a stand-in otherwise. </summary>
        /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/shobjidl_core/nf-shobjidl_core-ishellitemimagefactory-getimage#siigbf_memoryonly-0x00000002" />
        InMemoryOnly = 0x02,

        /// <summary> Only return icons. </summary>
        /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/shobjidl_core/nf-shobjidl_core-ishellitemimagefactory-getimage#siigbf_icononly-0x00000004"/>
        IconOnly = 0x04,

        /// <summary> Only return thumbnails. </summary>
        /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/shobjidl_core/nf-shobjidl_core-ishellitemimagefactory-getimage#siigbf_thumbnailonly-0x00000008"/>
        ThumbnailOnly = 0x08,

        /// <summary> Fetch the thumbnail from the thumbnail cache on disk if available or fallback to a stand-in otherwise. </summary>
        /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/shobjidl_core/nf-shobjidl_core-ishellitemimagefactory-getimage#siigbf_incacheonly-0x00000010"/>
        InCacheOnly = 0x10,
    }
}
