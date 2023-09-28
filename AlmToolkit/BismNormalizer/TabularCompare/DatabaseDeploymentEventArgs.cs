using System;

namespace BismNormalizer.TabularCompare
{
    /// <summary>
    /// Provides data for the Comparison.DatabaseDeployment event.
    /// </summary>
    public class DatabaseDeploymentEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets a flag indicating if the database deployment was successful.
        /// </summary>
        public bool DeploymentSuccessful { get; set; }
    }
}
