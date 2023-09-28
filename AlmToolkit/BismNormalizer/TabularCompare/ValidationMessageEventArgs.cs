using System;

namespace BismNormalizer.TabularCompare
{
    /// <summary>
    /// Provides data for the Comparison.ValidationMessage event.
    /// </summary>
    public class ValidationMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes ValidationMessageEventArgs with necessary data for Comparison.ValidationMessage event.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="validationMessageType">Type of object that a validation message relates to. For example, Table, Measure, MeasureCalculationDependency, etc.</param>
        /// <param name="validationMessageStatus">Status for a validation message, such as Informational and Warning.</param>
        public ValidationMessageEventArgs (string message, ValidationMessageType validationMessageType, ValidationMessageStatus validationMessageStatus)
        {
            Message = message;
            ValidationMessageType = validationMessageType;
            ValidationMessageStatus = validationMessageStatus;
        }

        /// <summary>
        /// Gets or sets the validation message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the ValidationMessageType.
        /// </summary>
        public ValidationMessageType ValidationMessageType { get; set; }

        /// <summary>
        /// Gets or sets the ValidationMessageStatus.
        /// </summary>
        public ValidationMessageStatus ValidationMessageStatus { get; set; }
    }
}
