using System;
using System.Linq;

namespace ChaosUtil.Platform.Paths
{
    /// <summary> Provides functions for file path normalization. </summary>
    public static class Normalization
    {
        /// <summary> Normalizes a path regardless of whether it's relative or absolute. </summary>
        /// <param name="path"> The path to be normalized. </param>
        /// <returns> The normalized path. </returns>
        public static string NormalizePath(string path)
            => string.Join("/", path
                .Split('/', '\\')
                .Select(f => f.Trim())
                .Where(f => !string.IsNullOrEmpty(f))
                );

        /// <summary>
        ///     Returns the normalized full path of the provided path (relative or absolute).
        ///     If the provided path is <see langword="null"/>, it is interpreted as an empty
        ///     path which results in returning the current working directory.
        /// </summary>
        /// <param name="path"> The path to be normalized. </param>
        /// <returns> The normalized full path. </returns>
        public static string NormalizeFullPath(string path)
            => NormalizePath(string.IsNullOrEmpty(path?.Trim())
                   ? Environment.CurrentDirectory
                   : System.IO.Path.GetFullPath(path)
               );

        /// <summary>
        ///     Normalizes the provided relative path.
        ///     If the provided path is <see langword="null"/>,
        ///     this function returns <see langword="null"/>.
        /// </summary>
        /// <param name="path"> The path to be normalized. </param>
        /// <returns>
        ///     The normalized relative path if the provided path is not <see langword="null"/>,
        ///     <see langword="null"/> otherwise.
        /// </returns>
        public static string NormalizeRelative(string path)
            => path == null ? null : NormalizePath(path);
    }
}
