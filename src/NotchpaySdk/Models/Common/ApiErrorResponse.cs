using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Serialization;

namespace NotchpaySdk.Models.Common;

/// <summary>
/// Represents an error response from the NotchPay API.
/// </summary>
public sealed record ApiErrorResponse
{
    /// <summary>
    /// Gets the error message from the API.
    /// </summary>
    [JsonPropertyName("message")]
    public required string Message { get; init; }

    /// <summary>
    /// Gets the validation errors, if any.
    /// The key is the field name, and the value is an array of error messages for that field.
    /// </summary>
    [JsonPropertyName("errors")]
    public Dictionary<string, string[]>? Errors { get; init; }
}
