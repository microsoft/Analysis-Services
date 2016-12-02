using System;

namespace AsPartitionProcessing
{
    /// <summary>
    /// Information required to connect to the configuration and logging database.
    /// </summary>
    public class ConfigDatabaseConnectionInfo
    {
        /// <summary>
        /// Database server name.
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// Name of the database.
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// User name used for connection.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Password used for connection.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Whether connection to be made using integrated authentication or SQL authentication.
        /// </summary>
        public bool IntegratedAuth { get; set; }
    }
}
