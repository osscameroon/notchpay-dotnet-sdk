using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NotchpaySdk.Models.Common;

/// <summary>
/// Represents pagination metadata for list responses.
/// </summary>
public sealed record PaginationMeta
{
    /// <summary>
    /// Gets the current page number.
    /// </summary>
    [JsonPropertyName("page")]
    public required int Page { get; init; }

    /// <summary>
    /// Gets the number of items per page.
    /// </summary>
    [JsonPropertyName("per_page")]
    public required int PerPage { get; init; }

    /// <summary>
    /// Gets the total number of items across all pages.
    /// </summary>
    [JsonPropertyName("total")]
    public required int Total { get; init; }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    [JsonIgnore]
    public int TotalPages => PerPage > 0 ? (int)System.Math.Ceiling((double)Total / PerPage) : 0;

    /// <summary>
    /// Gets a value indicating whether there is a next page.
    /// </summary>
    [JsonIgnore]
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Gets a value indicating whether there is a previous page.
    /// </summary>
    [JsonIgnore]
    public bool HasPreviousPage => Page > 1;
}

/// <summary>
/// Represents a paginated response from the API.
/// </summary>
/// <typeparam name="T">The type of items in the response.</typeparam>
public sealed record PaginatedResponse<T>
{
    /// <summary>
    /// Gets the list of items for the current page.
    /// </summary>
    [JsonPropertyName("data")]
    public required IReadOnlyList<T> Data { get; init; }

    /// <summary>
    /// Gets the pagination metadata.
    /// </summary>
    [JsonPropertyName("meta")]
    public required PaginationMeta Meta { get; init; }
}
