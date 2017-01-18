using System.Collections.Generic;

namespace Bitretsmah.UI.ConsoleApp
{
    public class ConsoleArguments
    {
        public bool ShowVersion { get; set; }
        public bool ShowInfo { get; set; }
        public List<string> SetAccountOld { get; set; }
        public List<string> SetDirectory { get; set; }

        /// <summary>
        ///     Creates new backup of a target
        /// </summary>
        /// <value>
        ///     The name of the target to backup.
        /// </value>
        /// <remarks>
        ///     bitretsmah --backup name
        /// </remarks>
        public string Backup { get; set; }

        /// <summary>
        ///     Restores backup of a target
        /// </summary>
        /// <value>
        ///     The name of the target to restore.
        /// </value>
        /// <remarks>
        ///     bitretsmah --restore name
        /// </remarks>
        public string Restore { get; set; }

        /// <summary>
        ///     Use name as username to setup the account.
        /// </summary>
        /// <value>
        ///     The account username
        /// </value>
        /// <remarks>
        ///     bitretsmah --set-account name [--password password]
        /// </remarks>
        public string SetAccount { get; set; }

        /// <summary>
        ///     Use password when setting account.
        /// </summary>
        /// <value>
        ///     The password
        /// </value>
        /// <remarks>
        ///     bitretsmah --password password
        /// </remarks>
        public string Password { get; set; }

        /// <summary>
        ///     Sets target
        /// </summary>
        /// <value>
        ///     The name of the target.
        /// </value>
        /// <remarks>
        ///     bitretsmah --set-target name --path path
        /// </remarks>
        public string SetTarget { get; set; }

        /// <summary>
        ///     Use name as path to local file or directory
        /// </summary>
        /// <value>
        ///     The path to local file or directory
        /// </value>
        /// <remarks>
        ///     bitretsmah --path name
        /// </remarks>
        public string Path { get; set; }

        /// <summary>
        ///     Suppress all warnings and confirmations.
        /// </summary>
        /// <remarks>
        ///     bitretsmah --forced
        /// </remarks>
        public bool Forced { get; set; }

        /// <summary>
        ///     Show help.
        /// </summary>
        /// <remarks>
        ///     bitretsmah --help
        /// </remarks>
        public bool Help { get; set; }

        /// <summary>
        ///     Show program version.
        /// </summary>
        /// <remarks>
        ///     bitretsmah --version
        /// </remarks>
        public bool Version { get; set; }
    }
}