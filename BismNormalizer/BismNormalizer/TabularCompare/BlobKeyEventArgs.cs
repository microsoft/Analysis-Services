using System;

namespace BismNormalizer.TabularCompare
{
    /// <summary>
    /// Initializes data for the Comparison.PasswordPrompt event.
    /// </summary>
    public class BlobKeyEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the authentication kind.
        /// </summary>
        public string AuthenticationKind { get; set; }

        /// <summary>
        /// Gets or sets the name of the data source.
        /// </summary>
        public string DataSourceName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string AccountKey { get; set; }

        /// <summary>
        /// Gets or sets the privacy level.
        /// </summary>
        public string PrivacyLevel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the user cancelled the deployment.
        /// </summary>
        public bool UserCancelled { get; set; }
    }
}
