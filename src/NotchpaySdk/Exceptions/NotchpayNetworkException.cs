using System;

namespace NotchpaySdk.Exceptions;

/// <summary>
/// Exception thrown when a network-level error occurs (timeout, connection failure, etc.).
/// </summary>
public sealed class NotchpayNetworkException : NotchpayException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotchpayNetworkException"/> class.
    /// </summary>
    /// <param name="message">The error message that explains the network issue.</param>
    public NotchpayNetworkException(string message)
        : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotchpayNetworkException"/> class.
    /// </summary>
    /// <param name="message">The error message that explains the network issue.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public NotchpayNetworkException(string message, Exception innerException)
        : base(message, innerException) { }
}
