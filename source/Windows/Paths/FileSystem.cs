using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using NTAccount = System.Security.Principal.NTAccount;

namespace ChaosUtil.Platform.Paths
{
    /// <summary> Provides utility functions for querying file system info. </summary>
    public static class FileSystem
    {
        /// <summary> The NTAccount name of the current Windows user. </summary>
        static readonly string currentUser = $@"{Environment.UserDomainName}\{Environment.UserName}";

        /// <summary> Checks whether the current Windows user has the specified access rights for a given file or directory. </summary>
        /// <param name="path"> The file or directory in question. </param>
        /// <param name="requestedRights"> The requested access rights. </param>
        /// <returns>
        ///     <see langword="true"/> if the current user has the requested rights,
        ///     <see langword="false"/> otherwise.
        /// </returns>
        public static bool HasAccess(string path, FileSystemRights requestedRights = FileSystemRights.FullControl)
        {
            try
            {
                DirectorySecurity access = new DirectoryInfo(path).GetAccessControl();
                AuthorizationRuleCollection rules = access.GetAccessRules(true, true, typeof(NTAccount));

                foreach (FileSystemAccessRule rule in rules.OfType<FileSystemAccessRule>())
                    if (rule.IdentityReference.Value.Equals(currentUser, StringComparison.CurrentCultureIgnoreCase))
                        if (rule.Affects(requestedRights))
                            if (rule.AccessControlType == AccessControlType.Deny)
                                return false;

            }
            catch (UnauthorizedAccessException)
            {
                // we may not even query the rights for this filesystem entry
                return false;
            }

            return true;
        }

        /// <summary> Check whether a specific rule affects any of the requested rights. </summary>
        /// <param name="rule"> The rule in question. </param>
        /// <param name="requestedRights"> The requested rights. </param>
        /// <returns>
        ///     <see langword="true"/> if the rule affects any of the requested rights,
        ///     <see langword="false"/> otherwise.
        /// </returns>
        static bool Affects(this FileSystemAccessRule rule, FileSystemRights requestedRights)
            => (rule.FileSystemRights & requestedRights) > 0;

#if COM_Supported
        /// <summary> Returns the path a shortcut leads to. </summary>
        /// <param name="shortcutFile"> The path to the shortcut file. </param>
        public static string GetShortcutTargetFile(string shortcutFile)
        {
            Shell32.FolderItem target = new Shell32.Shell()
                .NameSpace(Path.GetDirectoryName(shortcutFile))
                .ParseName(Path.GetFileName(shortcutFile));

            return target != null
                ? ((Shell32.ShellLinkObject)target.GetLink).Path
                : null;
        }
#endif
    }
}
