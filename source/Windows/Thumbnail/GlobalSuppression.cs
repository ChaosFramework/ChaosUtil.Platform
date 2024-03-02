using ChaosUtil.Platform.Windows.Thumbnail;
using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Compiler", "CS1591", // missing documentation
    Scope = "type",
    Target = "ChaosUtil.Platform.Windows.Thumbnail.ThumbnailOptions",
    Justification = SuppressMessageJustifications.THIRD_PARTY
    )]

[assembly: SuppressMessage("Compiler", "CS0168", // declared but not used
    Scope = "type",
    Target = "ChaosUtil.Platform.Windows.Thumbnail.WindowsThumbnailProvider",
    Justification = SuppressMessageJustifications.THIRD_PARTY
    )]

[assembly: SuppressMessage("Compiler", "CS1591", // missing documentation
    Scope = "type",
    Target = "ChaosUtil.Platform.Windows.Thumbnail.WindowsThumbnailProvider",
    Justification = SuppressMessageJustifications.THIRD_PARTY
    )]

[assembly: SuppressMessage("BadIdentifierCasing", "ChaosCC0102",
    Scope = "type",
    Target = "ChaosUtil.Platform.Windows.Thumbnail.NativeSize",
    Justification = SuppressMessageJustifications.THIRD_PARTY
    )]

namespace ChaosUtil.Platform.Windows.Thumbnail
{
    class SuppressMessageJustifications
    {
        internal const string THIRD_PARTY = "This is third party code following the coding style of the original author.";
    }
}
