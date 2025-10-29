using System;

namespace NotchpaySdk.Exceptions;

/// <summary>
/// Represents class for Notchpay .NET SDK errors that occur during application execution.
/// </summary>
public abstract class NotchpayException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotchpayException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected NotchpayException(string message)
        : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotchpayException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    protected NotchpayException(string message, Exception? innerException)
        : base(message, innerException) { }
}
