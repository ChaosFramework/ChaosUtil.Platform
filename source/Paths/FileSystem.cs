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
            // TODO: move implementations to ChaosUtil.Platform.* libraries
#if OS_WINDOWS
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
#else
#warning FileSystem.HasAccess is not implemented for this platform!
#endif
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
    }
}
