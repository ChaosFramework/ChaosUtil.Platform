using System;
using System.Text.RegularExpressions;
using StringBuilder = System.Text.StringBuilder;

namespace ChaosUtil.Platform.Paths
{
    /// <summary> Create <see cref="Regex"/> patterns from glob patterns. </summary>
    public static class GlobRegex
    {
        // TODO: fix ConvertGlobToRegex so that '**' matches everything
        /// <summary> A glob pattern that matches every relative file or directory path. </summary>
        public const string MATCH_ALL_GLOB = "**/*";

        // TODO: support '?' in glob
        // TODO: fix leading or trailing single asterisk matching at least one character (should also allow none)

        const string PATH_SEPARATOR = @"([/\\]+)";

        // TODO: Make this group actually match all characters allowed in path elements
        const string PATH_ELEMENT_DELIMITER = @"[a-zA-Z0-9\(\)_+\.-]";
        const string PATH_ELEMENT_INNER = "(" + PATH_ELEMENT_DELIMITER + "|[ ]" + ")";

        const string PATH_ELEMENT =
            "("
             + "("
                + PATH_ELEMENT_DELIMITER + "+"    // one or more delimiters
             + ")"
             + "|"                                // OR
             + "("
                + PATH_ELEMENT_DELIMITER          // start with delimiter
                + PATH_ELEMENT_INNER + "*"        // any number of anything
                + PATH_ELEMENT_DELIMITER          // end with delimiter
             + ")"
          + ")";

        const string DOUBLE_ASTERISK_REPLACE =
            "("
             + PATH_ELEMENT                       // one path element to start with
             + "("                                // any number of the following
                + PATH_SEPARATOR                  //     one or more path separators
                + PATH_ELEMENT                    //     plus another path element
             + ")*"
          + ")";

        const string START_ASTERISK_PATTERN = @"^\*";
        const string START_ASTERISK_REPLACE =
            "("
             + PATH_ELEMENT_DELIMITER
             + PATH_ELEMENT_INNER + "*"
          + ")";

        const string END_ASTERISK_PATTERN = @"\*$";
        const string END_ASTERISK_REPLACE =
            "("
             + PATH_ELEMENT_INNER + "*"
             + PATH_ELEMENT_DELIMITER
          + ")";

        const string MID_ASTERISK_PATTERN = @"\*";
        const string MID_ASTERISK_REPLACE =
            "("
             + PATH_ELEMENT_INNER + "*"
          + ")";

        const string DOUBLE_ASTERISK = @"\*\*";
        const string MULTI_DOUBLE_ASTERISK = "(" + DOUBLE_ASTERISK + PATH_SEPARATOR + DOUBLE_ASTERISK + ")";

        static readonly char[] pathSeparators = new[] { '/', '\\' };
        static readonly char[] needsEscape = new[] { '.', '(', ')', '[', ']', '{', '}', '$', '^' };

        static readonly Regex pathSeparatorRegex = new Regex(PATH_SEPARATOR, RegexOptions.Compiled);
        static readonly Regex multiDoubleAsteriskRegex = new Regex(MULTI_DOUBLE_ASTERISK, RegexOptions.Compiled);

        /// <summary>
        ///     Converts a glob pattern to a <see cref="Regex"/> pattern that matches the same paths as the glob pattern.
        ///     Matches absolute and relative paths that do not contain any wildcards.
        ///     If the glob patterns starts with a path separator, so will the <see cref="Regex"/> pattern.
        /// </summary>
        /// <param name="glob"> The glob pattern to be converted. </param>
        /// <returns> A regex pattern that can be fed to the <see cref="Regex(string)"/> constructor. </returns>
        public static string ConvertGlobToRegex(string glob)
        {
            Match firstPathSeparator = pathSeparatorRegex.Match(glob);
            bool startsWithPathSeparator = firstPathSeparator != null
                && firstPathSeparator.Success
                && firstPathSeparator.Index == 0;

            // reduce sequences like 'a/**/**/b' to 'a/**/b', to make path separator handling easier
            bool hasMultiDoubleAsterisk = true;
            while (hasMultiDoubleAsterisk)
            {
                string reduced = multiDoubleAsteriskRegex.Replace(glob, "**");
                hasMultiDoubleAsterisk = reduced != glob;
                glob = reduced;
            }

            string[] split = glob.Split(pathSeparators, StringSplitOptions.RemoveEmptyEntries);

            StringBuilder regex = new StringBuilder();

            int added = 0;
            for (int i = 0; i < split.Length; i++)
            {
                string element = split[i];

                switch (element)
                {
                    case ".":
                        continue; // the current directory. just ignore that. add no additional path separators either

                    case "**":
                        regex.Append('(');
                        if (added > 0 || startsWithPathSeparator)
                            regex.Append(PATH_SEPARATOR);
                        regex.Append(DOUBLE_ASTERISK_REPLACE);
                        regex.Append(")?");
                        added++;
                        break;

                    case "..":
                        throw new NotSupportedException("Going upwards is currently not supported.");

                    default:
                        if (added > 0 || startsWithPathSeparator)
                            regex.Append(PATH_SEPARATOR);
                        regex.Append(ReplaceWildCardsInSinglePathElement(element));
                        added++;
                        break;
                }
            }

            return $"^{regex}$";
        }

        static string ReplaceWildCardsInSinglePathElement(string element)
        {
            if (element.Contains("**"))
                throw new InvalidOperationException("Double asterisk may only occur as a whole path element.");

            if (element == "*")
                return PATH_ELEMENT;

            foreach (char c in needsEscape)
                element = element.Replace(c.ToString(), $@"\{c}");

            element = Regex.Replace(element, START_ASTERISK_PATTERN, START_ASTERISK_REPLACE); // replace leading *
            element = Regex.Replace(element, END_ASTERISK_PATTERN, END_ASTERISK_REPLACE);     // replace trailing *
            element = Regex.Replace(element, MID_ASTERISK_PATTERN, MID_ASTERISK_REPLACE);     // replace remaining *
            return $"({element})";
        }
    }
}
