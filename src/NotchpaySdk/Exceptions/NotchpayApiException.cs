using System;
using System.Collections.Generic;

namespace NotchpaySdk.Exceptions;

/// <summary>
/// Exception thrown when the NotchPay API returns an error response.
/// </summary>
public class NotchpayApiException : NotchpayException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotchpayApiException"/> class.
    /// </summary>
    /// <param name="message">The error message from the API.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public NotchpayApiException(string message, int statusCode)
        : base(message)
    {
        StatusCode = statusCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotchpayApiException"/> class.
    /// </summary>
    /// <param name="message">The error message from the API.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="requestId">The request ID from the API response.</param>
    public NotchpayApiException(string message, int statusCode, string? requestId)
        : base(message)
    {
        StatusCode = statusCode;
        RequestId = requestId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotchpayApiException"/> class.
    /// </summary>
    /// <param name="message">The error message from the API.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="validationErrors">The validation errors from the API.</param>
    /// <param name="requestId">The request ID from the API response.</param>
    public NotchpayApiException(
        string message,
        int statusCode,
        IReadOnlyDictionary<string, string[]>? validationErrors = null,
        string? requestId = null
    )
        : base(message)
    {
        StatusCode = statusCode;
        ValidationErrors = validationErrors;
        RequestId = requestId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotchpayApiException"/> class.
    /// </summary>
    /// <param name="message">The error message from the API.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="innerException">The inner exception.</param>
    public NotchpayApiException(string message, int statusCode, Exception innerException)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }

    /// <summary>
    /// Gets the HTTP status code returned by the API.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Gets the request ID from the API response, if available.
    /// </summary>
    public string? RequestId { get; }

    /// <summary>
    /// Gets the request ID from the API respone, if available.
    /// </summary>
    public IReadOnlyDictionary<string, string[]>? ValidationErrors { get; }
}
