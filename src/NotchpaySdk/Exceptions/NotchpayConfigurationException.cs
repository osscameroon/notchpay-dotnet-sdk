using System;

namespace NotchpaySdk.Exceptions;

/// <summary>
/// Exception thrown when the NotchPay SDK is configured incorrectly.
/// </summary>
public sealed class NotchpayConfigurationException : NotchpayException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotchpayConfigurationException"/> class.
    /// </summary>
    /// <param name="message">The error message that explains the configuration issue.</param>
    public NotchpayConfigurationException(string message)
        : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotchpayConfigurationException"/> class.
    /// </summary>
    /// <param name="message">The error message that explains the configuration issue.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public NotchpayConfigurationException(string message, Exception innerException)
        : base(message, innerException) { }
}
