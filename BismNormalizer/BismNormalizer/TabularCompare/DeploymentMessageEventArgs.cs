using System;

namespace BismNormalizer.TabularCompare
{
    /// <summary>
    /// Provides data for the IComparison.DeploymentMessage event.
    /// </summary>
    public class DeploymentMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes DeploymentMessageEventArgs with necessary data for IComparison.DeploymentMessage event.
        /// </summary>
        /// <param name="workItem">The work item that the message relates to.</param>
        /// <param name="message">The deployment message.</param>
        /// <param name="deploymentStatus">The DeploymentStatus.</param>
        public DeploymentMessageEventArgs(string workItem, string message, DeploymentStatus deploymentStatus)
        {
            WorkItem = workItem;
            Message = message;
            DeploymentStatus = deploymentStatus;
        }

        /// <summary>
        /// Gets or sets the work item that the message relates to.
        /// </summary>
        public string WorkItem { get; set; }

        /// <summary>
        /// Gets or set the deployment message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the deployment status.
        /// </summary>
        public DeploymentStatus DeploymentStatus { get; set; }
    }
}
