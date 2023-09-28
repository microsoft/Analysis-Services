using System;

namespace BismNormalizer.TabularCompare
{
    /// <summary>
    /// Provides data for the Comparison.DeploymentComplete event.
    /// </summary>
    public class DeploymentCompleteEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes DeploymentCompleteEventArgs with necessary data for Comparison.DeploymentComplete event.
        /// </summary>
        /// <param name="deploymentStatus">The deployment status.</param>
        /// <param name="errorMessage">The error message.</param>
        public DeploymentCompleteEventArgs(DeploymentStatus deploymentStatus, string errorMessage)
        {
            DeploymentStatus = deploymentStatus;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Gets or sets the deployment status.
        /// </summary>
        public DeploymentStatus DeploymentStatus { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
