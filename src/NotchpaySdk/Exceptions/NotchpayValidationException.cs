using System.Collections.Generic;
using System.Linq;

namespace NotchpaySdk.Exceptions;

/// <summary>
/// Exception thrown when client-side validation fails for a request.
/// </summary>
public sealed class NotchpayValidationException : NotchpayException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotchpayValidationException"/> class.
    /// </summary>
    /// <param name="errors">The validation errors.</param>
    public NotchpayValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base(BuildErrorMessage(errors))
    {
        Errors = errors;
    }

    /// <summary>
    /// Gets the validation errors.
    /// </summary>
    public IReadOnlyDictionary<string, string[]>? Errors { get; }

    private static string BuildErrorMessage(IReadOnlyDictionary<string, string[]>? errors)
    {
        if (errors == null || errors.Count == 0)
        {
            return "Request validation failed";
        }

        var errorMessages = errors.SelectMany(kvp => kvp.Value.Select(msg => $"{kvp.Key}: {msg}")).ToList();

        return $"Request validation failed: {string.Join("; ", errorMessages)}";
    }
}
